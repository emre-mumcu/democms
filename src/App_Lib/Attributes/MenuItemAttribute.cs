using System;
using System.Runtime.CompilerServices;

namespace src.App_Lib.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class MenuItemAttribute : Attribute
{
    public int? ParentIndex { get; private set; }
    public string? ParentIconClass { get; private set; }
    public string? ParentText { get; private set; }

    public int? MenuIndex { get; private set; }
    public string? MenuIconClass { get; private set; }
    public string? MenuText { get; private set; }

    public string? Link { get; private set; }
    public bool? IsSingle { get; set; }

    public MenuItemAttribute(
        bool _IsSingle = false,
        string? _Link = null,
        int _ParentIndex = 0,
        string? _ParentIconClass = null,
        string? _ParentText = null,
        int _MenuIndex = 0,
        string? _MenuIconClass = null,
        string? _MenuText = null,
        [CallerMemberName] string? _MethodOrPropertyName = null,
        [CallerFilePath] string? _SourceFilePath = null
        )
    {
        ParentIndex = _ParentIndex;

        ParentText = GetMenuDisplayName(_ParentText, _MenuText, _IsSingle, _SourceFilePath, _MethodOrPropertyName);

        ParentIconClass = _ParentIconClass;

        MenuIndex = _MenuIndex;

        MenuText = _MenuText ?? _MethodOrPropertyName;

        MenuIconClass = _MenuIconClass;

        Link = _Link;

        IsSingle = _IsSingle;
    }

    private string GetMenuDisplayName(string _MenuDisplayName, string _SubmenuDisplayName, bool _IsSingle, string _SourceFilePath, string MethodOrPropertyName)
    {
        if (!string.IsNullOrEmpty(_MenuDisplayName)) return _MenuDisplayName;
        else
        {
            if (!_IsSingle) return Path.GetFileNameWithoutExtension(_SourceFilePath)?.Replace("Controller", null);
            else
            {
                if (!string.IsNullOrEmpty(_SubmenuDisplayName)) return _SubmenuDisplayName;
                else
                {
                    return Path.GetFileNameWithoutExtension(_SourceFilePath)?.Replace("Controller", null) + MethodOrPropertyName;
                }
            }
        }
    }





}
