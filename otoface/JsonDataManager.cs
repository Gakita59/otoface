using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace otoface
{
    public class JsonDataManager
    {
        public ObservableCollection<Group> LoadGroups(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ObservableCollection<Group>>(json);
        }

        public void SaveGroups(string filePath, ObservableCollection<Group> groups)
        {
            string json = JsonConvert.SerializeObject(groups, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }

    public class IgnoreEmptyStringPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // プロパティの値が string 型で、空文字列の場合はシリアライズから除外する
            if (property.PropertyType == typeof(string))
            {
                property.ShouldSerialize = instance =>
                {
                    string value = (string)property.ValueProvider.GetValue(instance);
                    return !string.IsNullOrEmpty(value);
                };
            }

            return property;
        }
    }


}