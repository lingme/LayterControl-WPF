﻿using LayterControl.Model;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using System.Windows.Data;
using System.Windows.Controls;
using LayterControl.CommandExt;

namespace LayterControl
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowVM
    {
        /// <summary>
        /// 图层集合
        /// </summary>
        public ObservableCollection<ZLayter> LayterCollection { get; set; } = new ObservableCollection<ZLayter>();

        /// <summary>
        /// 选中图层
        /// </summary>
        public ZLayter SelectedZLayter { get; set; }

        /// <summary>
        /// 删除图层
        /// </summary>
        public ICommand CleanCommand
        {
            get
            {
                return new RelayCommand<ZLayter>((p)=> p.LayterSource = null);
            }
        }

        /// <summary>
        /// 选中图层
        /// </summary>
        public ICommand SelectedItemCommand
        {
            get
            {
                return new RelayCommand<TreeView>((p) =>
                {
                    if(p.SelectedItem is ZLayter zLayter)
                    {
                        SelectedZLayter = zLayter;
                        zLayter.LayterCheck = true;
                        foreach (var item in LayterCollection)
                        {
                            if (item.LayterID != zLayter.LayterID)
                                item.LayterCheck = false;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 方向键移动选中图层
        /// </summary>
        public ICommand KeyDownCommand
        {
            get
            {
                return new RelayCommand<CommandParameterExtend>((p) => 
                {
                    if(p.EventArgs is KeyEventArgs keyEventArgs)
                    {
                        if(SelectedZLayter != null && SelectedZLayter.CanMove)
                        {
                            switch(keyEventArgs.Key)
                            {
                                case Key.Left:
                                    SelectedZLayter.X = SelectedZLayter.X - 10;
                                    break;

                                case Key.Up:
                                    SelectedZLayter.Y = SelectedZLayter.Y - 10;
                                    break;

                                case Key.Right:
                                    SelectedZLayter.X = SelectedZLayter.X + 10;
                                    break;

                                case Key.Down:
                                    SelectedZLayter.Y = SelectedZLayter.Y + 10;
                                    break;
                            }
                            keyEventArgs.Handled = true;
                        }
                    }
                });
            }
        }


        public MainWindowVM()
        {
            for (int i = 0; i < 10; i++)
            {
                LayterCollection.Add(new ZLayter() {
                    LayterID = i,
                    LayterSource = new BitmapImage(new Uri(@"pack://application:,,,/Images/bottom.jpg")),
                    LayterName = $"图层 {i}",
                    Height = 250,
                    Width = 300,
                    X = i * 50,
                    Y = i * 50
                });
            }
        }
    }
}