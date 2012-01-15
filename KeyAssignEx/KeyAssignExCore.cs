using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Inscribe.Common;
using Inscribe.Configuration;
using Inscribe.Configuration.Settings;
using Inscribe.Storage;
using Inscribe.Subsystems;
using Inscribe.Subsystems.KeyAssign;

namespace KeyAssignEx
{
    static class KeyAssignExCore
    {
        private static bool changed = false;

        public static void Init()
        {
            KeyAssignCore.KeyAssignUpdated += (sender, e) =>
            {
                if (!changed)
                {
                    var lastException = ExceptionStorage.Exceptions.LastOrDefault();
                    if (lastException != null && lastException.Message.Contains("キーアサインファイル"))
                        ExceptionStorage.Remove(lastException);

                    SetAssignDescription(null);
                    if (!String.IsNullOrEmpty(Setting.Instance.KeyAssignProperty.KeyAssignFile) &&
                        Setting.Instance.KeyAssignProperty.KeyAssignFile != KeyAssignProperty.DefaultAssignFileName)
                    {
                        try
                        {
                            SetAssignDescription(LoadAssign(
                                AssignParser.Parse(KeyAssignHelper.GetPath(
                                    Setting.Instance.KeyAssignProperty.KeyAssignFile
                                ))
                            ));
                        }
                        catch (Exception ex)
                        {
                            ExceptionStorage.Register(ex, ExceptionCategory.ConfigurationError,
                                "キーアサインファイルを読み込めませんでした: " + Setting.Instance.KeyAssignProperty.KeyAssignFile);
                            SetAssignDescription(null);
                        }
                    }
                    if (GetAssignDescription() == null)
                    {
                        try
                        {
                            SetAssignDescription(LoadAssign(
                                AssignParser.Parse(KeyAssignHelper.GetPath(
                                    KeyAssignProperty.DefaultAssignFileName
                                ))
                            ));
                        }
                        catch (Exception ex)
                        {
                            ExceptionStorage.Register(ex, ExceptionCategory.InternalError,
                                "デフォルト キーアサインファイルが破損しています。Krileを再インストールしてください。");
                        }
                    }
                    
                    changed = true;
                    RaiseKeyAssignUpdated();
                }
                else
                {
                    changed = false;
                }
            };
        }

        private static AssignDescription GetAssignDescription()
        {
            return (AssignDescription)
                typeof(KeyAssignCore).InvokeMember(
                    "assignDescription",
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                    null,
                    null,
                    null
                );
        }

        private static void SetAssignDescription(AssignDescription value)
        {
            typeof(KeyAssignCore).InvokeMember(
                "assignDescription",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField,
                null,
                null,
                new object[] { value }
            );
        }

        private static void RaiseKeyAssignUpdated()
        {
            typeof(KeyAssignCore).InvokeMember(
                "OnKeyAssignUpdated",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod,
                null,
                null,
                new object[] { EventArgs.Empty }
            );
        }

        private static AssignDescription LoadAssign(XElement root)
        {
            return (AssignDescription)
                typeof(AssignLoader).GetMethod(
                    "LoadAssign",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new[] { typeof(XElement) },
                    null
                )
                .Invoke(null, new object[] { root });
        }
    }
}
