using System;
using System.Collections.Generic;
using System.Windows;
using WpfUiMenuItem = Wpf.Ui.Controls.MenuItem;

namespace FluentUsbTreeView.UIExtensions {
    public class RadioMenuItem : DependencyObject {
            public static Dictionary<WpfUiMenuItem, String> ElementToGroupNames = new Dictionary<WpfUiMenuItem, String>();

            public static readonly DependencyProperty GroupNameProperty =
               DependencyProperty.RegisterAttached("GroupName",
                                            typeof(String),
                                            typeof(RadioMenuItem),
                                            new PropertyMetadata(String.Empty, OnGroupNameChanged));

            public static void SetGroupName(WpfUiMenuItem element, String value) {
                element.SetValue(GroupNameProperty, value);
            }

            public static String GetGroupName(WpfUiMenuItem element) {
                return element.GetValue(GroupNameProperty).ToString();
            }

            private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
                //Add an entry to the group name collection
                var menuItem = d as WpfUiMenuItem;

                if ( menuItem != null ) {
                    String newGroupName = e.NewValue.ToString();
                    String oldGroupName = e.OldValue.ToString();
                    if ( String.IsNullOrEmpty(newGroupName) ) {
                        //Removing the toggle button from grouping
                        RemoveCheckboxFromGrouping(menuItem);
                    } else {
                        //Switching to a new group
                        if ( newGroupName != oldGroupName ) {
                            if ( !String.IsNullOrEmpty(oldGroupName) ) {
                                //Remove the old group mapping
                                RemoveCheckboxFromGrouping(menuItem);
                            }
                            ElementToGroupNames.Add(menuItem, e.NewValue.ToString());
                            menuItem.Checked += MenuItemChecked;
                        }
                    }
                }
            }

            private static void RemoveCheckboxFromGrouping(WpfUiMenuItem checkBox) {
                ElementToGroupNames.Remove(checkBox);
                checkBox.Checked -= MenuItemChecked;
            }


            static void MenuItemChecked(object sender, RoutedEventArgs e) {
                var menuItem = e.OriginalSource as WpfUiMenuItem;
                foreach ( var item in ElementToGroupNames ) {
                    if ( item.Key != menuItem && item.Value == GetGroupName(menuItem) ) {
                        item.Key.IsChecked = false;
                    }
                }
            }
        }
}
