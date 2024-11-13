#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YooAsset.Editor;

namespace ProxFramework.UI.Editor
{
    public class UIBindGenerateEditor : OdinEditorWindow
    {
        private const string KeyVariable = "//==自动化变量开始";
        private const string KeyPath = "//==自动化路径开始";
        
        [AssetsOnly] public GameObject uiSourceGameObject;

        [BoxGroup("Config")] [Sirenix.OdinInspector.FilePath(Extensions = "cs", RequireExistingPath = true)]
        public string uiWindowCodeTemplatePath;

        [BoxGroup("Config")] [Sirenix.OdinInspector.FilePath(Extensions = "cs", RequireExistingPath = true)]
        public string uiWindowGenTemplatePath;

        [BoxGroup("Config")] [FolderPath(RequireExistingPath = true)]
        public string uiCodeGeneratePath;

        [BoxGroup("Config")] public string uiCodeNamespace;
        
        [BoxGroup("Config")] [SerializeField] [InlineButton("SaveSetting")]
        public UIBindGenerateSetting setting;

        [BoxGroup("Result")] [HideLabel] [DisplayAsString]
        public string generateResult = "no result";

        [MenuItem("Prox/UI/Generate UIBind Code")]
        public static void ShowWindow()
        {
            GetWindow<UIBindGenerateEditor>("UIBind Generate");
        }

        protected override void Initialize()
        {
            setting = SettingLoader.LoadSettingData<UIBindGenerateSetting>();
            uiWindowCodeTemplatePath = setting.uiWindowCodeTemplatePath;
            uiWindowGenTemplatePath = setting.uiWindowGenTemplatePath;
            uiCodeGeneratePath = setting.uiCodeGeneratePath;
            uiCodeNamespace = setting.uiCodeNamespace;
        }

        [BoxGroup("Config")]
        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
        public void GenerateCode()
        {
            generateResult = "no result";
            var genBindResult = InternalGenerateBindCode();
            var genWindowResult = InternalGenerateWindowCode();
            generateResult =
                $"Generate Bind Code Result:{genBindResult} Generate Window Code Result:{genWindowResult}";
            SaveSetting();
        }

        private readonly Dictionary<string, Type> _nameTypesMap = new Dictionary<string, Type>()
        {
            { "Btn", typeof(Button) },
            { "BtnEx", typeof(ButtonEx) },
            { "Raw", typeof(RawImage) },
            { "Img", typeof(Image) },
            { "Text", typeof(TMP_Text) },
            { "Input", typeof(TMP_InputField) }
        };

        private readonly Dictionary<string, Type> _vNameTypesMap = new Dictionary<string, Type>();
        private readonly Dictionary<string, Type> _pathTypesMap = new Dictionary<string, Type>();
        private readonly Dictionary<string, string> _pathNameMap = new Dictionary<string, string>();

        private bool InternalGenerateBindCode()
        {
            _vNameTypesMap.Clear();
            _pathTypesMap.Clear();
            _pathNameMap.Clear();

            var children = uiSourceGameObject.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                var fullName = child.name;
                var tmpName = fullName.Split('_');
                if (tmpName.Length <= 1) continue;
                if (_nameTypesMap.TryGetValue(tmpName[0], out var type))
                {
                    var vName = tmpName[0].ToLower() + tmpName[1];
                    _vNameTypesMap.Add(vName, type);
                    //get child full path
                    var path = child.name;
                    var parent = child.parent;
                    while (parent != uiSourceGameObject.transform)
                    {
                        path = parent.name + "/" + path;
                        parent = parent.parent;
                    }

                    _pathTypesMap.Add(path, type);
                    _pathNameMap.Add(path, vName);
                    Debug.Log($"add {path} : {type}");
                }
            }

            if (_vNameTypesMap.Count == 0)
            {
                Debug.LogError("No UI Component Found");
                return false;
            }

            var streamReader = new StreamReader(uiWindowGenTemplatePath, Encoding.UTF8);
            var classText = streamReader.ReadToEnd();
            streamReader.Close();
            Debug.Log(classText);

            //生成
            //自动化变量
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("\n");
            foreach (var item in _vNameTypesMap)
            {
                stringBuilder.Append("        public " + item.Value.Name + " " + item.Key + "; ");
                stringBuilder.Append("\n");
            }

            classText = classText.Replace(KeyVariable, KeyVariable + stringBuilder.ToString());

            //自动化路径
            stringBuilder.Clear();
            stringBuilder.Append("\n");
            foreach (var item in _pathTypesMap)
            {
                var path = item.Key;
                var type = item.Value;
                var vName = _pathNameMap[path];
                stringBuilder.Append("            " + vName + " = Q<" + type.Name + ">(\"" + path + "\"); ");
                stringBuilder.Append("\n");
            }

            classText = classText.Replace(KeyPath, KeyPath + stringBuilder.ToString());

            //命名空间/类名
            if (!string.IsNullOrEmpty(uiCodeNamespace))
            {
                classText = classText.Replace("ProxFramework.UI.Template", uiCodeNamespace);
            }

            var className = uiSourceGameObject.name.Replace("P_", "");
            classText = classText.Replace("UIWindowTemplate", className);

            //写入
            var genPath =
                Application.dataPath.Replace("Assets", "") + $"{uiCodeGeneratePath}/{className}.Gen.cs";
            var streamWriter = new StreamWriter(genPath, false, Encoding.UTF8);
            streamWriter.Write(classText);
            streamWriter.Close();
            AssetDatabase.Refresh();
            return true;
        }

        private bool InternalGenerateWindowCode()
        {
            var streamReader = new StreamReader(uiWindowCodeTemplatePath, Encoding.UTF8);
            var classText = streamReader.ReadToEnd();
            //命名空间/类名
            if (!string.IsNullOrEmpty(uiCodeNamespace))
            {
                classText = classText.Replace("ProxFramework.UI.Template", uiCodeNamespace);
            }

            var className = uiSourceGameObject.name.Replace("P_", "");
            classText = classText.Replace("UIWindowTemplate", className);

            //写入
            var genPath =
                Application.dataPath.Replace("Assets", "") + $"{uiCodeGeneratePath}/{className}.cs";
            if (File.Exists(genPath))
            {
                Debug.LogWarning($"{genPath} is already exist!");
                return false;
            }

            var streamWriter = new StreamWriter(genPath, false, Encoding.UTF8);
            streamWriter.Write(classText);
            streamWriter.Close();
            AssetDatabase.Refresh();

            streamReader.Close();
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