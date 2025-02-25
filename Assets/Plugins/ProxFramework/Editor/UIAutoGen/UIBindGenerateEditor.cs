#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProxFramework.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YooAsset.Editor;

namespace ProxFramework.Editor
{
    public class UIBindGenerateEditor : EditorWindow
    {
        private const string KeyVariable = "//==自动化变量开始";
        private const string KeyPath = "//==自动化路径开始";

        public GameObject uiSourceGameObject;
        public string uiWindowCodeTemplatePath;
        public string uiWindowGenTemplatePath;
        public string uiCodeGeneratePath;
        public string uiCodeNamespace;
        public UIBindGenerateSetting setting;
        public string generateResult = "no result";

        [MenuItem("Prox/UI/Generate UIBind Code")]
        public static void ShowWindow()
        {
            GetWindow<UIBindGenerateEditor>("UIBind Generate");
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnGUI()
        {
            DrawUIFields();
            DrawButtons();
            EditorGUILayout.LabelField($"{generateResult}");
        }

        private void DrawUIFields()
        {
            uiSourceGameObject = (GameObject)EditorGUILayout.ObjectField("UI Source GameObject", uiSourceGameObject,
                typeof(GameObject), true);
            uiWindowCodeTemplatePath =
                EditorGUILayout.TextField("UI Window Code Template Path", uiWindowCodeTemplatePath);
            uiWindowGenTemplatePath = EditorGUILayout.TextField("UI Window Gen Template Path", uiWindowGenTemplatePath);
            uiCodeGeneratePath = EditorGUILayout.TextField("UI Code Generate Path", uiCodeGeneratePath);
            uiCodeNamespace = EditorGUILayout.TextField("UI Code Namespace", uiCodeNamespace);
            // setting = (UIBindGenerateSetting)EditorGUILayout.ObjectField("Setting", setting, typeof(UIBindGenerateSetting), false);
        }

        private void DrawButtons()
        {
            if (GUILayout.Button("Save Setting"))
            {
                SaveSetting();
            }

            if (GUILayout.Button("Generate Code"))
            {
                GenerateCode();
            }
        }

        private void Initialize()
        {
            setting = SettingLoader.LoadSettingData<UIBindGenerateSetting>();
            uiWindowCodeTemplatePath = setting.uiWindowCodeTemplatePath;
            uiWindowGenTemplatePath = setting.uiWindowGenTemplatePath;
            uiCodeGeneratePath = setting.uiCodeGeneratePath;
            uiCodeNamespace = setting.uiCodeNamespace;
        }

        private void GenerateCode()
        {
            generateResult = "no result";
            var genBindResult = InternalGenerateBindCode();
            var genWindowResult = false;
            if (genBindResult)
            {
                genWindowResult = InternalGenerateWindowCode();
            }

            generateResult = $"Generate Bind Code Result:{genBindResult} Generate Window Code Result:{genWindowResult}";
            SaveSetting();
        }

        private readonly Dictionary<string, Type> _nameTypesMap = new Dictionary<string, Type>()
        {
            { "@Dropdown", typeof(TMP_Dropdown) },
            { "@Btn", typeof(Button) },
            { "@BtnEx", typeof(ButtonEx) },
            { "@Raw", typeof(RawImage) },
            { "@Img", typeof(Image) },
            { "@Text", typeof(TMP_Text) },
            { "@Input", typeof(TMP_InputField) },
            { "@Obj", typeof(GameObject) }
        };

        private readonly Dictionary<string, Type> _vNameTypesMap = new Dictionary<string, Type>();
        private readonly Dictionary<string, Type> _pathTypesMap = new Dictionary<string, Type>();
        private readonly Dictionary<string, string> _pathNameMap = new Dictionary<string, string>();

        private bool InternalGenerateBindCode()
        {
            _vNameTypesMap.Clear();
            _pathTypesMap.Clear();
            _pathNameMap.Clear();
            if (uiSourceGameObject == null)
            {
                Debug.LogError("UI Source GameObject is null");
                return false;
            }

            var children = uiSourceGameObject.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                ProcessChild(child);
            }

            if (_vNameTypesMap.Count == 0)
            {
                Debug.LogWarning("No UI Component Found");
            }

            var classText = ReadTemplate(uiWindowGenTemplatePath);
            classText = ReplacePlaceholders(classText);

            var className = uiSourceGameObject.name.Replace("P_", "");
            classText = classText.Replace("UIWindowTemplate", className);

            WriteToFile($"{uiCodeGeneratePath}/{className}.Gen.cs", classText);
            return true;
        }

        private void ProcessChild(Transform child)
        {
            var fullName = child.name;
            var tmpName = fullName.Split('_');
            if (tmpName.Length <= 1) return;
            if (_nameTypesMap.TryGetValue(tmpName[0], out var type))
            {
                var vName = tmpName[0].ToLower() + tmpName[1];
                _vNameTypesMap.Add(vName, type);
                var path = GetFullPath(child);
                _pathTypesMap.Add(path, type);
                _pathNameMap.Add(path, vName);
                Debug.Log($"add {path} : {type}");
            }
        }

        private string GetFullPath(Transform child)
        {
            var path = child.name;
            var parent = child.parent;
            while (parent != uiSourceGameObject.transform)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        private string ReadTemplate(string templatePath)
        {
            using (var streamReader = new StreamReader(templatePath, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        private string ReplacePlaceholders(string classText)
        {
            var variableBuilder = new StringBuilder();
            variableBuilder.Append("\n");
            foreach (var item in _vNameTypesMap)
            {
                variableBuilder.Append($"        public {item.Value.Name} {item.Key}; \n");
            }

            classText = classText.Replace(KeyVariable, KeyVariable + variableBuilder.ToString());

            var pathBuilder = new StringBuilder();
            pathBuilder.Append("\n");
            foreach (var item in _pathTypesMap)
            {
                var path = item.Key;
                var type = item.Value;
                var vName = _pathNameMap[path];
                if (type == typeof(GameObject))
                {
                    pathBuilder.Append($"            {vName} = Q(\"{path}\").gameObject; \n");
                }
                else
                {
                    pathBuilder.Append($"            {vName} = Q<{type.Name}>(\"{path}\"); \n");
                }
            }

            classText = classText.Replace(KeyPath, KeyPath + pathBuilder.ToString());

            if (!string.IsNullOrEmpty(uiCodeNamespace))
            {
                classText = classText.Replace("ProxFramework.UI.Template", uiCodeNamespace);
            }

            return classText;
        }

        private void WriteToFile(string path, string content)
        {
            var fullPath = Application.dataPath.Replace("Assets", "") + path;
            using (var streamWriter = new StreamWriter(fullPath, false, Encoding.UTF8))
            {
                streamWriter.Write(content);
            }

            AssetDatabase.Refresh();
        }

        private bool InternalGenerateWindowCode()
        {
            var classText = ReadTemplate(uiWindowCodeTemplatePath);

            if (!string.IsNullOrEmpty(uiCodeNamespace))
            {
                classText = classText.Replace("ProxFramework.UI.Template", uiCodeNamespace);
            }

            var className = uiSourceGameObject.name.Replace("P_", "");
            classText = classText.Replace("UIWindowTemplate", className);

            var genPath = $"{uiCodeGeneratePath}/{className}.cs";
            if (File.Exists(Application.dataPath.Replace("Assets", "") + genPath))
            {
                Debug.LogWarning($"{genPath} is already exist! skip generate");
                return false;
            }

            WriteToFile(genPath, classText);
            return true;
        }

        private void SaveSetting()
        {
            setting.uiWindowCodeTemplatePath = uiWindowCodeTemplatePath;
            setting.uiWindowGenTemplatePath = uiWindowGenTemplatePath;
            setting.uiCodeGeneratePath = uiCodeGeneratePath;
            setting.uiCodeNamespace = uiCodeNamespace;
            EditorUtility.SetDirty(setting);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif