using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AttributeBinder.Helpers;

namespace AttributeBinder
{
    public class AttryBinder
    {

        #region Static Inicialization 

        private static Action<Action> RunOnMainThread { get; set; }

        public static void SetRunOnMainThread(Action<Action> runOnMainThreadAction)
        {
            RunOnMainThread = runOnMainThreadAction;
        }

        #endregion

        private WeakReference<object> _weakTarget;

        protected Dictionary<string, string> PropertiesMapper { get; set; }

        public AttryBinder(object target)
        {
            PropertiesMapper = new Dictionary<string, string>();
            _weakTarget = new WeakReference<object>(target);
        }

        public virtual object Target
        {
            get
            {
                if (_weakTarget != null && _weakTarget.TryGetTarget(out var target))
                {
                    return target;
                }
                return null;
            }

            set
            {
                _weakTarget.SetTarget(value);
            }
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
#if DEBUG
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif
            if (PropertiesMapper.TryGetValue(propertyName, out var p))
            {
                var value = sender.GetPropValue(propertyName);
#if DEBUG
                if (RunOnMainThread != null)
                {
#endif
                    RunOnMainThread.Invoke(() => Target.SetPropValue(p, value));
#if DEBUG
                }
                else
                {
                    //TODO Explain on readme how RunOnUIThread works
                    throw new Exception("Please set the RunOnMainThread property. Read Readme.txt for more information");
                }
#endif
            }

#if DEBUG
            stopWatch.Stop();
            System.Diagnostics.Debug.WriteLine($"AttryBinder OnPropertyChanged Ellapsed tick:{stopWatch.ElapsedTicks} millis:{stopWatch.ElapsedMilliseconds}");
#endif
        }


        #region Inicialization

        public void CreateAttributeMappers()
        {
#if DEBUG
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif
            Type targetType = Target.GetType();
            PropertiesMapper = CreatePropertiesMapper(targetType);
#if DEBUG
            stopWatch.Stop();
            System.Diagnostics.Debug.WriteLine($"AttryBinder OnPropertyChanged Ellapsed tick:{stopWatch.ElapsedTicks} millis:{stopWatch.ElapsedMilliseconds}");
#endif
        }

        private static Dictionary<string, string> CreatePropertiesMapper(Type targetType)
        {
            var targetProperties = targetType.GetProperties();

            var propertiesMapper = new Dictionary<string, string>();
            foreach (PropertyInfo p in targetProperties)
            {
                object[] v = p.GetCustomAttributes(inherit: false);
                foreach (Bind b in v.Where(x => x is Bind))
                {
                    string sourcePropertyName = b.Source;
                    propertiesMapper[sourcePropertyName] = p.Name;
                }
            }
            return propertiesMapper;
        }
    }

    #endregion

}

