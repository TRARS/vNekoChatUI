﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace vNekoChatUI.CustomControlEx.ConsoleListViewEx
{
    partial class cConsoleListView_event
    {
        private void Item_PreviewMouseLeftButtonDown(object s, MouseButtonEventArgs e)
        {
            SetTextToClipboard(((ListViewItem)s).DataContext.ToString());
        }

        private void Item_PreviewMouseRightButtonDown(object s, MouseButtonEventArgs e)
        {
            SetTextToClipboard(((ListViewItem)s).DataContext.ToString());
        }

        private void SetTextToClipboard(string? obj)
        {
            if (obj is null) { return; }

            System.Threading.Thread sta_thread = new System.Threading.Thread(() =>
            {
                try { Clipboard.SetText(obj); }
                catch (Exception ex) { MessageBox.Show($"{ex.Message}"); }
            });
            sta_thread.SetApartmentState(System.Threading.ApartmentState.STA);
            sta_thread.Start();
        }
    }
}
