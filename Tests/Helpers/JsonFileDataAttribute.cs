using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace GeneticOptimizationFramework.Tests.Helpers
{
    public class JsonFileDataAttribute : DataAttribute
    {
        private readonly string _filePath;
        private readonly string _propertyName;
        private readonly Type _dataType;

        public JsonFileDataAttribute(string filePath)
            : this(filePath, null, null) { }

        public JsonFileDataAttribute(string filePath, string propertyName, Type dataType)
        {
            _filePath = filePath;
            _propertyName = propertyName;
            _dataType = dataType;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null)
            {
                throw new ArgumentNullException(nameof(testMethod));
            }
            
            var path = Path.IsPathRooted(_filePath)
                ? _filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            var fileData = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(_propertyName))            
            {
                return GetData(fileData);
            }
            else
            {
                var allData = JObject.Parse(fileData);
                var data = allData[_propertyName].ToString();
                return GetData(data);
            }
        }

        private IEnumerable<object[]> GetData(string jsonData)
        {
            var generic = typeof(List<>).MakeGenericType(_dataType);
            dynamic datalist = JsonConvert.DeserializeObject(jsonData, generic);

            var objectList = new List<object[]>();
            foreach (var data in datalist)
            {
                objectList.Add(new object[] { data });
            }

            return objectList;
        }
    }
}
