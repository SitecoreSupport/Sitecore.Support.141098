# Sitecore.Support.141098

When using a custom session state provider (for example `mongo`, `mssql` or `redis`), the `RebuildAll`, `Rebuild` and `RefreshTree`  commands in the `Developer` tab may throw the `SerializationException`.

An example of the exception and its stack trace is:

```
ERROR Application error.
Exception: System.Web.HttpException
Message: Unable to serialize the session state. In 'StateServer' and 'SQLServer' mode, ASP.NET will serialize the session state objects, and as a result non-serializable objects or MarshalByRef objects are not permitted. The same restriction applies if similar serialization is done by the custom session state store in 'Custom' mode.
Source: System.Web
   at System.Web.Util.AltSerialization.WriteValueToStream(Object value, BinaryWriter writer)
   at System.Web.SessionState.SessionStateItemCollection.WriteValueToStreamWithAssert(Object value, BinaryWriter writer)
   at System.Web.SessionState.SessionStateItemCollection.Serialize(BinaryWriter writer)
   at Sitecore.SessionProvider.SessionStateSerializer.Serialize(BinaryWriter writer, SessionStateStoreData item)
   at Sitecore.SessionProvider.SessionStateSerializer.Compress(SessionStateStoreData item)
   at Sitecore.SessionProvider.SessionStateSerializer.Serialize(SessionStateStoreData sessionState, Boolean compress)
   at Sitecore.SessionProvider.Sql.SqlSessionStateStore.UpdateAndReleaseItem(Guid application, String id, String lockCookie, SessionStateActions action, SessionStateStoreData sessionState)
   at Sitecore.SessionProvider.Sql.SqlSessionStateProvider.SetAndReleaseItemExclusive(HttpContext context, String id, SessionStateStoreData sessionState, Object lockId, Boolean newItem)
   at System.Web.SessionState.SessionStateModule.OnReleaseState(Object source, EventArgs eventArgs)
   at System.Web.HttpApplication.SyncEventExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()
   at System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)

Nested Exception

Exception: System.Runtime.Serialization.SerializationException
Message: Type 'Sitecore.Abstractions.TranslateWrapper' in Assembly 'Sitecore.Abstractions, Version=10.0.0.0, Culture=neutral, PublicKeyToken=null' is not marked as serializable.
Source: mscorlib
   at System.Runtime.Serialization.FormatterServices.InternalGetSerializableMembers(RuntimeType type)
   at System.Runtime.Serialization.FormatterServices.GetSerializableMembers(Type type, StreamingContext context)
   at System.Runtime.Serialization.Formatters.Binary.WriteObjectInfo.InitMemberInfo()
   at System.Runtime.Serialization.Formatters.Binary.WriteObjectInfo.InitSerialize(Object obj, ISurrogateSelector surrogateSelector, StreamingContext context, SerObjectInfoInit serObjectInfoInit, IFormatterConverter converter, ObjectWriter objectWriter, SerializationBinder binder)
   at System.Runtime.Serialization.Formatters.Binary.ObjectWriter.Write(WriteObjectInfo objectInfo, NameInfo memberNameInfo, NameInfo typeNameInfo)
   at System.Runtime.Serialization.Formatters.Binary.ObjectWriter.Serialize(Object graph, Header[] inHeaders, __BinaryWriter serWriter, Boolean fCheck)
   at System.Runtime.Serialization.Formatters.Binary.BinaryFormatter.Serialize(Stream serializationStream, Object graph, Header[] headers, Boolean fCheck)
   at System.Web.Util.AltSerialization.WriteValueToStream(Object value, BinaryWriter writer)
```

## License  
This patch is licensed under the [Sitecore Corporation A/S License for GitHub](https://github.com/sitecoresupport/Sitecore.Support.141098/blob/master/LICENSE).  

## Download  
Downloads are available via [GitHub Releases](https://github.com/sitecoresupport/Sitecore.Support.141098/releases).  
