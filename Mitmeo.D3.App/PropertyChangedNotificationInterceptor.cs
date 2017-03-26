using Mitmeo.D3.App.ViewModels;
using System;

namespace Mitmeo.D3.App
{
    public static class PropertyChangedNotificationInterceptor
    {
        public static void Intercept(object target, Action onPropertyChangedAction, string propertyName)
        {
            onPropertyChangedAction();

            var sendKey = target as SendKeyViewModel;
            if (sendKey != null)
            {
                if (propertyName == "Enabled")
                {
                    if (sendKey.Enabled)
                    {
                        sendKey.Run();
                    }
                    else
                    {
                        sendKey.Clear();
                    }
                }
            }
        }
    }
}
