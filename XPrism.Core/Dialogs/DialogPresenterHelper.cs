using System.Reflection;

namespace XPrism.Core.Dialogs;

public static class DialogPresenterHelper {
    private static Dictionary<Type, Type?> DialogBaseTypes = new Dictionary<Type, Type?>();

    public static void RegisterDialogType<T, TD>() {
        DialogBaseTypes[typeof(T)] = typeof(TD);
    }

    public static Type? GetDialogType(Type dialogBaseType) {
        if (DialogBaseTypes.TryGetValue(dialogBaseType, out Type? dialogBase))
        {
            return dialogBase;
        }

        throw new Exception("No dialog base type found: dialog view not registered");
    }
}