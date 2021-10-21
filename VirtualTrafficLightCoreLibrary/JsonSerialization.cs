using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace   VirtualTrafficLightCoreLibrary
{
    public static class JsonSerialization
    {
        static readonly JsonSerializer _serializer;
        static readonly JsonSerializerSettings _settings;
        static JsonSerialization( )
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver 
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = false
                    }
                }   
            };
            _settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            _serializer = JsonSerializer.Create( _settings );

            _serializer.Error += delegate (object sender, ErrorEventArgs args)
            {
                if (args.CurrentObject == args.ErrorContext.OriginalObject)
                {
                }
            };
        }
        public static JObject Serialize( object @object ) => JObject.FromObject( @object, _serializer );
        public static JObject Deserialize( string json ) => JObject.Parse( json );
        public static TMessage Deserialize<TMessage>( JObject jobject ) => jobject.ToObject<TMessage>( );
 
        public static object ToObject( Type type, JObject source ) => source.ToObject( type );
    }
}
