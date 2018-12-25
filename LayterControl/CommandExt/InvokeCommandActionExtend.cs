using Microsoft.Xaml.Behaviors;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;


namespace LayterControl.CommandExt
{
    /// <summary>
    /// InvokeCommandAction扩展类
    /// </summary>
    public class InvokeCommandActionExtend : TriggerAction<DependencyObject>
    {
        private string commandName;
        /// <summary>
        /// 获得或设置此操作应调用的命令的名称。  
        /// </summary>
        /// <value>此操作应调用的命令的名称。</value>  
        /// <remarks>如果设置了此属性和 Command 属性，则此属性将被后者所取代。</remarks>  
        public string CommandName
        {
            get { base.ReadPreamble(); return commandName; }
            set
            {
                if (this.CommandName != value)
                {
                    base.WritePreamble();
                    this.commandName = value;
                    base.WritePostscript();
                }
            }
        }

        /// <summary>  
        /// 获取或设置此操作应调用的命令。这是依赖属性。  
        /// </summary>  
        /// <value>要执行的命令。</value>  
        /// <remarks>如果设置了此属性和 CommandName 属性，则此属性将优先于后者。</remarks>  
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandActionExtend), new PropertyMetadata(null));

        /// <summary>  
        /// 获得或设置命令参数。这是依赖属性。  
        /// </summary>  
        /// <value>命令参数。</value>  
        /// <remarks>这是传递给 ICommand.CanExecute 和 ICommand.Execute 的值。</remarks>  
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandActionExtend), new PropertyMetadata(null));

        /// <summary>  
        /// 调用操作。  
        /// </summary>  
        /// <param name="parameter">操作的参数。如果操作不需要参数，则可以将参数设置为空引用。</param>  
        protected override void Invoke(object parameter)
        {
            ICommand command = this.ResolveCommand();
            CommandParameterExtend parameterExtend = new CommandParameterExtend() // 添加事件触发源和事件参数 
            {
                Sender = base.AssociatedObject,
                Parameter = GetValue(CommandParameterProperty),
                EventArgs = parameter as EventArgs
            };
            if (command != null && command.CanExecute(parameterExtend))
                command.Execute(parameterExtend); // 将扩展的参数传递到Execute方法中 
        }

        /// <summary>
        /// 解析Command
        /// </summary>
        /// <returns> Command </returns>
        private ICommand ResolveCommand()
        {
            ICommand result = null;
            if (this.Command != null)
                result = this.Command;
            else
            {
                if (base.AssociatedObject != null)
                {
                    Type type = base.AssociatedObject.GetType();
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropertyInfo[] array = properties;
                    for (int i = 0; i < array.Length; i++)
                    {
                        PropertyInfo propertyInfo = array[i];
                        if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType) && string.Equals(propertyInfo.Name, this.CommandName, StringComparison.Ordinal))
                            result = (ICommand)propertyInfo.GetValue(base.AssociatedObject, null);
                    }
                }
            }
            return result;
        }
    }


    /// <summary>  
    /// CommandParameter扩展类，使CommandParameter可以带事件参数  
    /// </summary>  
    public class CommandParameterExtend
    {
        /// <summary>  
        /// 事件触发源  
        /// </summary>  
        public DependencyObject Sender { get; set; }

        /// <summary>  
        /// 事件参数  
        /// </summary>  
        public EventArgs EventArgs { get; set; }

        /// <summary>  
        /// 额外参数  
        /// </summary>  
        public object Parameter { get; set; }
    }
}
