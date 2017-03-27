using Mitmeo.D3.App.ViewModels.Base;
using System;

namespace Mitmeo.D3.App
{
    public static class PropertyChangedNotificationInterceptor
    {
        public static void Intercept(object target, Action onPropertyChangedAction, string propertyName)
        {
            onPropertyChangedAction();

            var model = target as ViewModelBase;
            if (model != null)
            {
                if (propertyName == "Enabled")
                {
                    if (model.Enabled)
                    {
                        model.AfterEnabled();
                    }
                    else
                    {
                        model.AfterDisabled();
                    }
                }
            }
        }
    }
}
