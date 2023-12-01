using System;
using System.Windows;

namespace PicturesEditor.Core.DragnDrop
{
    internal sealed class DragnDropHelper
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(DragnDropHelper),
                new FrameworkPropertyMetadata(default(bool), OnEnabledChanged) { BindsTwoWayByDefault = false });

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached("Target", typeof(object), typeof(DragnDropHelper), null);

        public static bool GetIsEnabled(DependencyObject obj) =>
            (bool)obj.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject obj, bool value) =>
            obj.SetValue(IsEnabledProperty, value);

        public static object GetTarget(DependencyObject obj) =>
            obj.GetValue(TargetProperty);

        public static void SetTarget(DependencyObject obj, object value) =>
            obj.SetValue(TargetProperty, value);

        private static void OnEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (sender is UIElement ui)
                {
                    if ((bool)e.NewValue)
                    {
                        ui.AllowDrop = true;
                        ui.Drop += OnDrop;
                    }
                    else
                    {
                        ui.AllowDrop = false;
                        ui.Drop -= OnDrop;
                    }
                }
            }
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (sender is DependencyObject d)
            {
                object target = d.GetValue(TargetProperty);
                if (target is IFileDragnDropTarget fileTarget)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        fileTarget.OnFileDrop(files: (string[])e.Data.GetData(DataFormats.FileDrop));
                    }
                }
                else
                    throw new InvalidOperationException("Объект Target должен реализовывать интерфейс " + nameof(IFileDragnDropTarget));
            }
        }
    }
}
