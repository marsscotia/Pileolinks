# Event Handler Memory Leak Audit Report

## Summary
This audit identified **5 critical memory leaks** and **2 moderate issues** related to improper event handler registration/de-registration in the Pileolinks application.

---

## CRITICAL ISSUES (Memory Leaks)

### 1. **TreeFlyout.xaml.cs - No Event Handler Cleanup on Disposal**
**Severity**: 🔴 CRITICAL

**Location**: `Pileolinks/TreeFlyout.xaml.cs` (lines 13-18)

**Issue**: 
Event handlers are registered in the constructor but are **NEVER unregistered**:
- `ViewModel.DeleteRequested`
- `ViewModel.AlertRequested`
- `ViewModel.AddCollectionRequested`
- `ViewModel.AddDirectoryRequested`
- `ViewModel.RenameRequested`

**Why This Leaks Memory**:
- TreeFlyout is a ContentPage that gets pushed/popped from navigation
- When it's dismissed, these handlers keep the ViewModel alive
- The ViewModel holds references to services and collections
- These services may hold data in memory indefinitely

**Fix Required**: Add cleanup in `OnDisappearing()` override

---

### 2. **MainPage.xaml.cs - Navigation Event Handlers Not Fully Cleaned Up**
**Severity**: 🔴 CRITICAL

**Location**: `Pileolinks/MainPage.xaml.cs` (lines 13-17, 57-66)

**Issue**:
Multiple event handler leaks:
1. `treeFlyout.TreeItemViewModelChanged` registered in constructor (line 14) - **NEVER unregistered**
2. `treeFlyout.Loaded` registered in constructor (line 15) - **NEVER unregistered**
3. Navigation page events are registered on-the-fly in `TreeNavigation_Pushed()` (lines 30-31) but while they are cleaned up in `TreeNavigation_Popped()`, there's incomplete cleanup in `OnAppearing()` (lines 57-66)

**Why This Leaks Memory**:
- FlyoutPage instances may be created multiple times
- These handlers prevent garbage collection of the TreeFlyout
- The navigation history accumulates these handlers

**Fix Required**: Add `OnDisappearing()` override to unregister the two constructor-level handlers

---

### 3. **DirectoryView.xaml.cs - Multiple Event Handlers Never Unregistered**
**Severity**: 🔴 CRITICAL

**Location**: `Pileolinks/Views/DirectoryView.xaml.cs` (lines 16-21)

**Issue**:
Five event handlers are registered in the constructor but **NEVER unregistered**:
- `ViewModel.DirectorySelected`
- `ViewModel.EditLinkRequested`
- `ViewModel.LaunchUrlRequested`
- `ViewModel.AlertRequested`
- `ViewModel.SearchRequested`
- `ViewModel.ConfirmDeleteLinkRequested`

**Why This Leaks Memory**:
- DirectoryView is pushed/popped from navigation multiple times during app usage
- Each instance left on the navigation stack keeps its handlers
- The ViewModel instances are held alive by these handlers
- Memory accumulates as users browse the directory tree

**Fix Required**: Add cleanup in `OnDisappearing()` override

---

### 4. **Tree.xaml.cs - TreeItemViewModel Event Handlers Not Cleaned Up on Delete**
**Severity**: 🔴 CRITICAL

**Location**: `Pileolinks/Components/Tree/Tree.xaml.cs` (lines 156-221)

**Issue**:
In the `ItemDeleted()` method, event handlers are properly unregistered (lines 167-169), BUT there are orphaned handlers in `AddDescendant()` (line 236-239) and `PopulateItems()` (line 244-247):
- Handlers are registered to TreeItemViewModel events: `DescendantAdded`, `Deleted`, `ItemMoved`
- When items are deleted, only the handlers on the deleted items are removed
- But newly added descendants also accumulate handlers without corresponding removal logic in some paths

**Also Critical**: In `AddTopLevelDirectory()` (lines 66-88), event handlers are added but not registered in a way that ensures cleanup if the directory is later removed before `ItemDeleted()` is called.

**Why This Leaks Memory**:
- As users add and remove directories, orphaned handlers accumulate
- Each handler holds a reference to the TreeItemViewModel
- TreeItemViewModels hold references to ITreeItem objects which may contain links and data

**Fix Required**: Ensure all registration paths have corresponding unregistration

---

### 5. **SearchViewModel.xaml.cs - Results_CollectionChanged Handler Never Unregistered**
**Severity**: 🔴 CRITICAL

**Location**: `Pileolinks/ViewModels/SearchViewModel.cs` (line 91)

**Issue**:
In the constructor, `Results.CollectionChanged` is registered but **NEVER unregistered**:
```csharp
Results.CollectionChanged += Results_CollectionChanged;
```

The `Leaving()` method clears handlers on individual LinkViewModels but NOT the collection handler.

**Why This Leaks Memory**:
- SearchViewModel instances may be reused or recreated
- The collection handler keeps the Results collection alive
- Multiple search operations could accumulate instances in memory
- The Results collection holds LinkViewModel objects which hold model data

**Fix Required**: Add cleanup in a disposal method or `Leaving()` method

---

## MODERATE ISSUES

### 6. **SearchViewModel.xaml.cs - Incomplete Handler Cleanup in ClearSearch()**
**Severity**: 🟡 MODERATE

**Location**: `Pileolinks/ViewModels/SearchViewModel.cs` (lines 180-188)

**Issue**:
In `ClearSearch()`, not all handlers are unregistered:
```csharp
foreach (var result in Results)
{
	result.LaunchUrlRequested -= LinkViewModel_LaunchUrlRequested;
	result.EditLinkRequested -= LinkViewModel_EditLinkRequested;
	// Missing: result.CopyUrlRequested and result.OpenParentRequested
}
```

**Why This Is a Problem**:
- If the user performs multiple searches with clears, handlers can accumulate
- The `CopyUrlRequested` and `OpenParentRequested` handlers are registered but not cleaned here

**Fix Required**: Add cleanup for all four handlers

---

### 7. **MainPage.xaml.cs - OnAppearing() Registers Multiple Handlers Without Full Cleanup**
**Severity**: 🟡 MODERATE

**Location**: `Pileolinks/MainPage.xaml.cs` (lines 57-66)

**Issue**:
In `OnAppearing()`, a new DirectoryView is created and handlers are registered:
```csharp
initial.NavigatedTo += TreeNavigation_NavigatedTo;
initial.NavigatedFrom += TreeNavigation_NavigatedFrom;
```

These are registered but if `OnAppearing()` is called multiple times (which can happen when returning from background), new handlers are added without removing old ones.

**Why This Is a Problem**:
- App backgrounding/foreground cycling can cause OnAppearing() to be called multiple times
- Handlers accumulate on the same initial page

**Fix Required**: Check if handlers already exist before registering, or clean them up properly

---

## SUMMARY TABLE

| File | Issue Type | Handler(s) | Line(s) | Priority |
|------|-----------|-----------|--------|----------|
| TreeFlyout.xaml.cs | No cleanup | DeleteRequested, AlertRequested, Add/RenameRequested | 13-18 | CRITICAL |
| MainPage.xaml.cs | No cleanup | TreeItemViewModelChanged, Loaded | 14-15 | CRITICAL |
| MainPage.xaml.cs | Potential re-registration | NavigatedTo, NavigatedFrom | 57-66 | MODERATE |
| DirectoryView.xaml.cs | No cleanup | 6 handlers | 16-21 | CRITICAL |
| Tree.xaml.cs | Incomplete cleanup path | DescendantAdded, Deleted, ItemMoved | 66-247 | CRITICAL |
| SearchViewModel.cs | No cleanup | Results.CollectionChanged | 91 | CRITICAL |
| SearchViewModel.cs | Incomplete cleanup | CopyUrlRequested, OpenParentRequested | 180-188 | MODERATE |

---

## RECOMMENDATIONS

1. **Immediate Action**: Implement `OnDisappearing()` cleanup in TreeFlyout, DirectoryView
2. **High Priority**: Add disposal method or enhance `Leaving()` in SearchViewModel
3. **High Priority**: Fix Tree.xaml.cs event handler registration tracking
4. **Review**: MainPage.xaml.cs navigation handler lifecycle
5. **Long-term**: Consider implementing `IDisposable` pattern for ViewModels that manage event handlers
6. **Best Practice**: Use weak event patterns or helper methods for consistent registration/unregistration

