﻿

#pragma checksum "C:\Users\Asif\Documents\Visual Studio 2013\Projects\Budgeting\Budgeting\Views\GraphDisplayPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E493FB2B7B2BFDA3E54DAF43F3FD380E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Budgeting.Views
{
    partial class GraphDisplayPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 54 "..\..\Views\GraphDisplayPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 57 "..\..\Views\GraphDisplayPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListPickerFlyout)(target)).ItemsPicked += this.GraphEventItemsPicked;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 162 "..\..\Views\GraphDisplayPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListPickerFlyout)(target)).ItemsPicked += this.TermChosen;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 106 "..\..\Views\GraphDisplayPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.StartKeyGraph_Clicked;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 81 "..\..\Views\GraphDisplayPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.StartGraph_Clicked;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 72 "..\..\Views\GraphDisplayPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListPickerFlyout)(target)).ItemsPicked += this.SBChooserItemsPicked;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

