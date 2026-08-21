#region Copyright (c) 2003-2005, Luke T. Maxon : (Contributed by Ian Cooper) : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' Modernisation 2026-2026 Smurf.IV
' All rights reserved.
' 
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
' 
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
' 
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
' 
' * Neither the name of the author nor the names of its contributors may be used to endorse or 
' 	promote products derived from this software without specific prior written permission.
' 
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
' ******************************************************************************************************************/

#endregion

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.TestApplications.TestForms;

public class ToolbarTestForm : Form
{
    private IContainer components;
    private ContextMenu contextMenuColor;

    private ImageList imageListToolbar;

    private Label labelToolbarSelection;
    private MenuItem menuItemBlue;
    private MenuItem menuItemGreen;

    private MenuItem menuItemIndigo;
    private MenuItem menuItemOrange;
    private MenuItem menuItemRed;
    private MenuItem menuItemViolet;
    private MenuItem menuItemYellow;

    private ToolBarButton toolBarButtonClose;
    private ToolBarButton toolBarButtonColorPicker;
    private ToolBarButton toolBarButtonNext;

    private ToolBarButton toolBarButtonOpen;
    private ToolBarButton toolBarButtonPrevious;
    private ToolBarButton toolBarButtonSeperator;
    private ToolBarButton toolBarButtonSeperator2;
    private ToolBarButton toolBarButtonToggle;
    private ToolBar toolBarTest;

    public ToolbarTestForm()
    {
        InitializeComponent();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }

    private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
    {
        labelToolbarSelection.Text = e.Button.Text;
    }

    private void menuItemRed_Click(object sender, EventArgs e)
    {
        OnMenuItemClick((MenuItem) sender);
    }

    private void menuItemOrange_Click(object sender, EventArgs e)
    {
        OnMenuItemClick((MenuItem) sender);
    }

    private void menuItemYellow_Click(object sender, EventArgs e)
    {
        OnMenuItemClick((MenuItem) sender);
    }

    private void menuItemGreen_Click(object sender, EventArgs e)
    {
        OnMenuItemClick((MenuItem) sender);
    }

    private void menuItemBlue_Click(object sender, EventArgs e)
    {
        OnMenuItemClick((MenuItem) sender);
    }

    private void menuItemIndigo_Click(object sender, EventArgs e)
    {
        OnMenuItemClick((MenuItem) sender);
    }

    private void menuItemViolet_Click(object sender, EventArgs e)
    {
        OnMenuItemClick((MenuItem) sender);
    }

    private void OnMenuItemClick(MenuItem sender)
    {
        labelToolbarSelection.Text = sender.Text;
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new Container();
        toolBarButtonOpen = new ToolBarButton();
        menuItemGreen = new MenuItem();
        toolBarButtonClose = new ToolBarButton();
        toolBarButtonColorPicker = new ToolBarButton();
        menuItemIndigo = new MenuItem();
        toolBarButtonPrevious = new ToolBarButton();
        menuItemOrange = new MenuItem();
        toolBarButtonNext = new ToolBarButton();
        menuItemRed = new MenuItem();
        toolBarButtonSeperator = new ToolBarButton();
        menuItemYellow = new MenuItem();
        contextMenuColor = new ContextMenu();
        toolBarTest = new ToolBar();
        toolBarButtonSeperator2 = new ToolBarButton();
        labelToolbarSelection = new Label();
        menuItemViolet = new MenuItem();
        imageListToolbar = new ImageList(components);
        menuItemBlue = new MenuItem();
        toolBarButtonToggle = new ToolBarButton();
        SuspendLayout();
        // 
        // toolBarButtonOpen
        // 
        toolBarButtonOpen.ImageIndex = 0;
        toolBarButtonOpen.Text = "Open";
        toolBarButtonOpen.ToolTipText = "Open File";
        // 
        // menuItemGreen
        // 
        menuItemGreen.Index = 3;
        menuItemGreen.Text = "Green";
        menuItemGreen.Click += new EventHandler(menuItemGreen_Click);
        // 
        // toolBarButtonClose
        // 
        toolBarButtonClose.ImageIndex = 1;
        toolBarButtonClose.Text = "Close";
        toolBarButtonClose.ToolTipText = "Close File";
        // 
        // toolBarButtonColorPicker
        // 
        toolBarButtonColorPicker.DropDownMenu = contextMenuColor;
        toolBarButtonColorPicker.ImageIndex = 4;
        toolBarButtonColorPicker.Style = ToolBarButtonStyle.DropDownButton;
        toolBarButtonColorPicker.Text = "Color";
        toolBarButtonColorPicker.ToolTipText = "Pick a Color";
        // 
        // menuItemIndigo
        // 
        menuItemIndigo.Index = 5;
        menuItemIndigo.Text = "Indigo";
        menuItemIndigo.Click += new EventHandler(menuItemIndigo_Click);
        // 
        // toolBarButtonPrevious
        // 
        toolBarButtonPrevious.ImageIndex = 3;
        toolBarButtonPrevious.Text = "Previous";
        toolBarButtonPrevious.ToolTipText = "Previous Record";
        // 
        // menuItemOrange
        // 
        menuItemOrange.Index = 1;
        menuItemOrange.Text = "Orange";
        menuItemOrange.Click += new EventHandler(menuItemOrange_Click);
        // 
        // toolBarButtonNext
        // 
        toolBarButtonNext.ImageIndex = 2;
        toolBarButtonNext.Text = "Next";
        toolBarButtonNext.ToolTipText = "Next Record";
        // 
        // menuItemRed
        // 
        menuItemRed.Index = 0;
        menuItemRed.Text = "Red";
        menuItemRed.Click += new EventHandler(menuItemRed_Click);
        // 
        // toolBarButtonSeperator
        // 
        toolBarButtonSeperator.Style = ToolBarButtonStyle.Separator;
        // 
        // menuItemYellow
        // 
        menuItemYellow.Index = 2;
        menuItemYellow.Text = "Yellow";
        menuItemYellow.Click += new EventHandler(menuItemYellow_Click);
        // 
        // contextMenuColor
        // 
        contextMenuColor.MenuItems.AddRange(
            new MenuItem[]
            {
                menuItemRed, menuItemOrange, menuItemYellow, menuItemGreen,
                menuItemBlue, menuItemIndigo, menuItemViolet
            });
        // 
        // toolBarTest
        // 
        toolBarTest.Buttons.AddRange(
            new ToolBarButton[]
            {
                toolBarButtonOpen, toolBarButtonClose, toolBarButtonNext,
                toolBarButtonPrevious, toolBarButtonSeperator, toolBarButtonColorPicker,
                toolBarButtonSeperator2, toolBarButtonToggle
            });
        toolBarTest.DropDownArrows = true;
        toolBarTest.ImageList = imageListToolbar;
        toolBarTest.Location = new System.Drawing.Point(0, 0);
        toolBarTest.Name = "toolBarTest";
        toolBarTest.ShowToolTips = true;
        toolBarTest.Size = new System.Drawing.Size(352, 50);
        toolBarTest.TabIndex = 0;
        toolBarTest.ButtonClick +=
            new ToolBarButtonClickEventHandler(toolBar1_ButtonClick);
        // 
        // toolBarButtonSeperator2
        // 
        toolBarButtonSeperator2.Style = ToolBarButtonStyle.Separator;
        // 
        // labelToolbarSelection
        // 
        labelToolbarSelection.Location = new System.Drawing.Point(80, 80);
        labelToolbarSelection.Name = "labelToolbarSelection";
        labelToolbarSelection.TabIndex = 1;
        labelToolbarSelection.Text = "label1";
        // 
        // menuItemViolet
        // 
        menuItemViolet.Index = 6;
        menuItemViolet.Text = "Violet";
        menuItemViolet.Click += new EventHandler(menuItemViolet_Click);
        // 
        // imageListToolbar
        // 
        imageListToolbar.ImageSize = new System.Drawing.Size(16, 16);
        imageListToolbar.TransparentColor = System.Drawing.Color.Transparent;
        // 
        // menuItemBlue
        // 
        menuItemBlue.Index = 4;
        menuItemBlue.Text = "Blue";
        menuItemBlue.Click += new EventHandler(menuItemBlue_Click);
        // 
        // toolBarButtonToggle
        // 
        toolBarButtonToggle.ImageIndex = 5;
        toolBarButtonToggle.Style = ToolBarButtonStyle.ToggleButton;
        // 
        // ToolbarTestForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(5, 14);
        ClientSize = new System.Drawing.Size(352, 272);
        Controls.Add(labelToolbarSelection);
        Controls.Add(toolBarTest);
        Name = "ToolbarTestForm";
        Text = "ToolbarTestForm";
        ResumeLayout(false);
    }

    #endregion
}
#endif