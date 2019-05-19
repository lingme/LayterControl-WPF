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


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandActionExtend), new PropertyMetadata(null));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandActionExtend), new PropertyMetadata(null));


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

    public class CommandParameterExtend
    {
        public DependencyObject Sender { get; set; }

        public EventArgs EventArgs { get; set; }

        public object Parameter { get; set; }
    }
}
