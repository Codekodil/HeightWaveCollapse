//Generated with https://github.com/Codekodil/UnhedderCodeGen
namespace HeightWaveCollapseBase{
internal static class SharedPointer{[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]public static extern IntPtr Wrapper_Shared_Ptr_Get(IntPtr self);}
internal class NativeException:Exception{private NativeException(string message):base(message){}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern int Wrapper_Get_Exception(IntPtr buffer,int maxSize);[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]public static unsafe Exception GetNative(){fixed(byte*buffer=stackalloc byte[128]){return System.Text.Encoding.ASCII.GetString(buffer,Wrapper_Get_Exception((IntPtr)buffer,128))switch{nameof(NullReferenceException)=>new NullReferenceException(),nameof(ArgumentException)=>new ArgumentException(),nameof(ArgumentNullException)=>new ArgumentNullException(),nameof(ArgumentOutOfRangeException)=>new ArgumentOutOfRangeException(),var message => new NativeException(message)};}}}
internal abstract class Wrapper{public IntPtr?Native;public virtual IntPtr MemoryLocation=>Native??throw new ObjectDisposedException(GetType().Name);}
internal abstract class SharedWrapper:Wrapper{[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)][System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]public static extern IntPtr Wrapper_Shared_Ptr_Get(IntPtr self);override public IntPtr MemoryLocation=>Wrapper_Shared_Ptr_Get(base.MemoryLocation);}
}
namespace HeightWaveCollapseBase.HeightWaveCollapseBase{
internal class CellInitializer:Wrapper,IDisposable{public CellInitializer(IntPtr?native){Native=native;}
public unsafe CellInitializer(){try{Native=Wrapper_New_HeightWaveCollapseBase_CellInitializer_0();}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern IntPtr Wrapper_New_HeightWaveCollapseBase_CellInitializer_0();
public unsafe HeightWaveCollapseBase.WaveList? Generate(int x,int y){try{IntPtr value_result;value_result=Wrapper_Call_HeightWaveCollapseBase_CellInitializer_Generate_0(Native??throw new System.ObjectDisposedException(nameof(CellInitializer)),x,y);return HeightWaveCollapseBase.WaveList._lookup.GetOrMake(value_result);}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern IntPtr Wrapper_Call_HeightWaveCollapseBase_CellInitializer_Generate_0(IntPtr self,int x_,int y_);
private delegate IntPtr InitCell_Delegate_Native(int x_,int y_);private InitCell_Delegate_Native? InitCell_Delegate_Native_Object;[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase",CallingConvention=System.Runtime.InteropServices.CallingConvention.StdCall)]private static extern void Wrapper_Event_HeightWaveCollapseBase_CellInitializer_InitCell(IntPtr self, InitCell_Delegate_Native? action);public delegate IntPtr InitCellDelegate(int x,int y);private InitCellDelegate? InitCellDelegate_Object;public event InitCellDelegate InitCell{add{try{InitCellDelegate_Object+=value;if(InitCell_Delegate_Native_Object==null){InitCell_Delegate_Native_Object=Delegate;unsafe IntPtr Delegate(int x_,int y_)=>InitCellDelegate_Object?.Invoke(x_,y_)??default;Wrapper_Event_HeightWaveCollapseBase_CellInitializer_InitCell(Native??throw new System.ObjectDisposedException(nameof(CellInitializer)),InitCell_Delegate_Native_Object);}}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}remove{InitCellDelegate_Object-=value;}}
public void Dispose()=>Wrapper_Delete();~CellInitializer()=>Wrapper_Delete();private void Wrapper_Delete(){try{if(!Native.HasValue)return;if(InitCell_Delegate_Native_Object!=null){Wrapper_Event_HeightWaveCollapseBase_CellInitializer_InitCell(Native.Value,null);InitCell_Delegate_Native_Object=null;}Wrapper_Delete_HeightWaveCollapseBase_CellInitializer(Native.Value);Native=default;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern void Wrapper_Delete_HeightWaveCollapseBase_CellInitializer(IntPtr native);
}}
namespace HeightWaveCollapseBase.HeightWaveCollapseBase{
internal class CellCollapse:Wrapper,IDisposable{public CellCollapse(IntPtr?native){Native=native;}
public unsafe CellCollapse(){try{Native=Wrapper_New_HeightWaveCollapseBase_CellCollapse_0();}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern IntPtr Wrapper_New_HeightWaveCollapseBase_CellCollapse_0();
private delegate void CollapseCell_Delegate_Native(int x_,int y_,IntPtr id_,IntPtr height_);private CollapseCell_Delegate_Native? CollapseCell_Delegate_Native_Object;[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase",CallingConvention=System.Runtime.InteropServices.CallingConvention.StdCall)]private static extern void Wrapper_Event_HeightWaveCollapseBase_CellCollapse_CollapseCell(IntPtr self, CollapseCell_Delegate_Native? action);public delegate void CollapseCellDelegate(int x,int y,ref int id,ref int height);private CollapseCellDelegate? CollapseCellDelegate_Object;public event CollapseCellDelegate CollapseCell{add{try{CollapseCellDelegate_Object+=value;if(CollapseCell_Delegate_Native_Object==null){CollapseCell_Delegate_Native_Object=Delegate;unsafe void Delegate(int x_,int y_,IntPtr id_,IntPtr height_)=>CollapseCellDelegate_Object?.Invoke(x_,y_,ref System.Runtime.CompilerServices.Unsafe.AsRef<int>((void*)id_),ref System.Runtime.CompilerServices.Unsafe.AsRef<int>((void*)height_));Wrapper_Event_HeightWaveCollapseBase_CellCollapse_CollapseCell(Native??throw new System.ObjectDisposedException(nameof(CellCollapse)),CollapseCell_Delegate_Native_Object);}}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}remove{CollapseCellDelegate_Object-=value;}}
public void Dispose()=>Wrapper_Delete();~CellCollapse()=>Wrapper_Delete();private void Wrapper_Delete(){try{if(!Native.HasValue)return;if(CollapseCell_Delegate_Native_Object!=null){Wrapper_Event_HeightWaveCollapseBase_CellCollapse_CollapseCell(Native.Value,null);CollapseCell_Delegate_Native_Object=null;}Wrapper_Delete_HeightWaveCollapseBase_CellCollapse(Native.Value);Native=default;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern void Wrapper_Delete_HeightWaveCollapseBase_CellCollapse(IntPtr native);
}}
namespace HeightWaveCollapseBase.HeightWaveCollapseBase{
internal class WaveField:Wrapper,IDisposable{public WaveField(IntPtr?native){Native=native;}
internal class WaveFieldPointerLookup:PointerLookup<WaveField>{protected override WaveField New(IntPtr ptr)=>new WaveField((IntPtr?)ptr);protected override void Delete(IntPtr ptr)=>Wrapper_Delete_HeightWaveCollapseBase_WaveField(ptr);}internal static readonly WaveFieldPointerLookup _lookup=new WaveFieldPointerLookup();
public unsafe WaveField(int chunkWidth,int chunkHeight){try{Native=Wrapper_New_HeightWaveCollapseBase_WaveField_0(chunkWidth,chunkHeight);_lookup.Add(this,Native.Value);}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern IntPtr Wrapper_New_HeightWaveCollapseBase_WaveField_0(int chunkWidth_,int chunkHeight_);
public unsafe bool AddChunk(int chunkX,int chunkY,HeightWaveCollapseBase.CellInitializer? initializer){try{bool value_result;value_result=Wrapper_Call_HeightWaveCollapseBase_WaveField_AddChunk_0(Native??throw new System.ObjectDisposedException(nameof(WaveField)),chunkX,chunkY,(initializer==null?IntPtr.Zero:initializer!.Native??throw new System.ObjectDisposedException(nameof(initializer))));return value_result;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern bool Wrapper_Call_HeightWaveCollapseBase_WaveField_AddChunk_0(IntPtr self,int chunkX_,int chunkY_,IntPtr initializer_);
public unsafe HeightWaveCollapseBase.WaveList? ListAt(int x,int y){try{IntPtr value_result;value_result=Wrapper_Call_HeightWaveCollapseBase_WaveField_ListAt_1(Native??throw new System.ObjectDisposedException(nameof(WaveField)),x,y);return HeightWaveCollapseBase.WaveList._lookup.GetOrMake(value_result);}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern IntPtr Wrapper_Call_HeightWaveCollapseBase_WaveField_ListAt_1(IntPtr self,int x_,int y_);
public unsafe void Collapse(HeightWaveCollapseBase.WaveFunction? func,HeightWaveCollapseBase.CellCollapse? collapse){try{Wrapper_Call_HeightWaveCollapseBase_WaveField_Collapse_2(Native??throw new System.ObjectDisposedException(nameof(WaveField)),(func==null?IntPtr.Zero:func!.Native??throw new System.ObjectDisposedException(nameof(func))),(collapse==null?IntPtr.Zero:collapse!.Native??throw new System.ObjectDisposedException(nameof(collapse))));return ;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern void Wrapper_Call_HeightWaveCollapseBase_WaveField_Collapse_2(IntPtr self,IntPtr func_,IntPtr collapse_);
public void Dispose()=>Wrapper_Delete();~WaveField()=>Wrapper_Delete();private void Wrapper_Delete(){try{if(!Native.HasValue)return;Wrapper_Delete_HeightWaveCollapseBase_WaveField(Native.Value);Native=default;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern void Wrapper_Delete_HeightWaveCollapseBase_WaveField(IntPtr native);
}}
namespace HeightWaveCollapseBase.HeightWaveCollapseBase{
internal class WaveFunction:Wrapper,IDisposable{public WaveFunction(IntPtr?native){Native=native;}
public unsafe WaveFunction(int possibilities){try{Native=Wrapper_New_HeightWaveCollapseBase_WaveFunction_0(possibilities);}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern IntPtr Wrapper_New_HeightWaveCollapseBase_WaveFunction_0(int possibilities_);
public unsafe bool SetPossibilities(int index,HeightWaveCollapseBase.WaveList? left,HeightWaveCollapseBase.WaveList? up,HeightWaveCollapseBase.WaveList? right,HeightWaveCollapseBase.WaveList? down){try{bool value_result;value_result=Wrapper_Call_HeightWaveCollapseBase_WaveFunction_SetPossibilities_0(Native??throw new System.ObjectDisposedException(nameof(WaveFunction)),index,(left==null?IntPtr.Zero:left!.Native??throw new System.ObjectDisposedException(nameof(left))),(up==null?IntPtr.Zero:up!.Native??throw new System.ObjectDisposedException(nameof(up))),(right==null?IntPtr.Zero:right!.Native??throw new System.ObjectDisposedException(nameof(right))),(down==null?IntPtr.Zero:down!.Native??throw new System.ObjectDisposedException(nameof(down))));return value_result;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern bool Wrapper_Call_HeightWaveCollapseBase_WaveFunction_SetPossibilities_0(IntPtr self,int index_,IntPtr left_,IntPtr up_,IntPtr right_,IntPtr down_);
public void Dispose()=>Wrapper_Delete();~WaveFunction()=>Wrapper_Delete();private void Wrapper_Delete(){try{if(!Native.HasValue)return;Wrapper_Delete_HeightWaveCollapseBase_WaveFunction(Native.Value);Native=default;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern void Wrapper_Delete_HeightWaveCollapseBase_WaveFunction(IntPtr native);
}}
namespace HeightWaveCollapseBase.HeightWaveCollapseBase{
internal class WaveList:Wrapper,IDisposable{public WaveList(IntPtr?native){Native=native;}
internal class WaveListPointerLookup:PointerLookup<WaveList>{protected override WaveList New(IntPtr ptr)=>new WaveList((IntPtr?)ptr);protected override void Delete(IntPtr ptr)=>Wrapper_Delete_HeightWaveCollapseBase_WaveList(ptr);}internal static readonly WaveListPointerLookup _lookup=new WaveListPointerLookup();
public unsafe WaveList(int size){try{Native=Wrapper_New_HeightWaveCollapseBase_WaveList_0(size);_lookup.Add(this,Native.Value);}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern IntPtr Wrapper_New_HeightWaveCollapseBase_WaveList_0(int size_);
public unsafe int GetSize(){try{int value_result;value_result=Wrapper_Call_HeightWaveCollapseBase_WaveList_GetSize_0(Native??throw new System.ObjectDisposedException(nameof(WaveList)));return value_result;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern int Wrapper_Call_HeightWaveCollapseBase_WaveList_GetSize_0(IntPtr self);
public unsafe bool Get(int index,ref int id,ref int height){try{bool value_result;fixed(int*local6541=&id){fixed(int*local6542=&height){value_result=Wrapper_Call_HeightWaveCollapseBase_WaveList_Get_1(Native??throw new System.ObjectDisposedException(nameof(WaveList)),index,(IntPtr)local6541,(IntPtr)local6542);}}return value_result;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern bool Wrapper_Call_HeightWaveCollapseBase_WaveList_Get_1(IntPtr self,int index_,IntPtr id_,IntPtr height_);
public unsafe bool Set(int index,int id,int height){try{bool value_result;value_result=Wrapper_Call_HeightWaveCollapseBase_WaveList_Set_2(Native??throw new System.ObjectDisposedException(nameof(WaveList)),index,id,height);return value_result;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern bool Wrapper_Call_HeightWaveCollapseBase_WaveList_Set_2(IntPtr self,int index_,int id_,int height_);
public void Dispose()=>Wrapper_Delete();~WaveList()=>Wrapper_Delete();private void Wrapper_Delete(){try{if(!Native.HasValue)return;Wrapper_Delete_HeightWaveCollapseBase_WaveList(Native.Value);Native=default;}catch(System.Runtime.InteropServices.SEHException){throw NativeException.GetNative();}}[System.Runtime.InteropServices.DllImport("HeightWaveCollapseBase")]private static extern void Wrapper_Delete_HeightWaveCollapseBase_WaveList(IntPtr native);
}}
namespace HeightWaveCollapseBase{internal class _SafeGuard{private readonly object _locker = new object();private int _callCount = 0;private TaskCompletionSource? _disposeTask;private Action _delete;public _SafeGuard(Action delete)=>_delete = delete;public DisposableLock Lock(string objectName)=>new DisposableLock(this, objectName);public Task DeleteAsync(){var doDelete=false;lock(_locker){if(_disposeTask==null){_disposeTask=new TaskCompletionSource();doDelete=_callCount==0;}}if(doDelete){try{_delete();_disposeTask.SetResult();}catch(Exception e){_disposeTask.SetException(e);}}return _disposeTask.Task;}public struct DisposableLock:IDisposable{private readonly _SafeGuard _guard;public DisposableLock(_SafeGuard guard,string objectName){_guard = guard;lock(guard._locker){if(guard._disposeTask!=null)throw new ObjectDisposedException(objectName);guard._callCount++;}}public void Dispose(){var doDelete=false;lock(_guard._locker){if(--_guard._callCount==0)doDelete=_guard._disposeTask!=null;}if(doDelete){try{_guard._delete();_guard._disposeTask!.SetResult();}catch(Exception e){_guard._disposeTask!.SetException(e);}}}}}}
namespace HeightWaveCollapseBase{internal abstract class PointerLookup<T>where T:Wrapper{protected abstract T New(IntPtr ptr);protected abstract void Delete(IntPtr ptr);private readonly Dictionary<IntPtr,WeakReference<T>>_references=new Dictionary<IntPtr,WeakReference<T>>();private readonly bool _shared=typeof(SharedWrapper).IsAssignableFrom(typeof(T));public PointerLookup(){PeriodicGC();async void PeriodicGC(){while(true){await Task.Delay(10000);lock(_references){var toRemove=new List<IntPtr>();foreach(var kv in _references)if(!kv.Value.TryGetTarget(out _))toRemove.Add(kv.Key);foreach(var remove in toRemove)_references.Remove(remove);}}}}public T GetOrMake(IntPtr ptr){lock(_references){var memory=_shared?SharedPointer.Wrapper_Shared_Ptr_Get(ptr):ptr;if(!(_references.TryGetValue(memory,out var weak)&&weak.TryGetTarget(out var t)&&t.Native.HasValue))_references[memory]=new WeakReference<T>(t=New(ptr));else if(_shared)Delete(ptr);return t;}}public void Add(T reference,IntPtr ptr){lock(_references)_references[_shared?SharedPointer.Wrapper_Shared_Ptr_Get(ptr):ptr]=new WeakReference<T>(reference);}}}
