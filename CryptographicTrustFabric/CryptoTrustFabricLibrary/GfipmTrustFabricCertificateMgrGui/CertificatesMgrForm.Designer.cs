//
// Copyright (c) 2012, Georgia Institute of Technology. All Rights Reserved.
// This code was developed by Georgia Tech Research Institute (GTRI) under
// a grant from the U.S. Dept. of Justice, Bureau of Justice Assistance.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CryptoTrustFabricMgr
{
    partial class CertificatesMgrForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CertificatesMgrForm));
            this.buttonInstall = new System.Windows.Forms.Button();
            this.buttonUninstall = new System.Windows.Forms.Button();
            this.buttonUninstallAll = new System.Windows.Forms.Button();
            this.InstallTo = new System.Windows.Forms.GroupBox();
            this.installToLocalComputerRadioButton = new System.Windows.Forms.RadioButton();
            this.installToUserRadioButton = new System.Windows.Forms.RadioButton();
            this.buttonInstallAll = new System.Windows.Forms.Button();
            this.listInstalledCertificates = new System.Windows.Forms.ListView();
            this.columnEntityIdSelected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEntityTypeSelected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnKeyUseSelected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSubjectSelected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listAvailableCertificates = new System.Windows.Forms.ListView();
            this.columnEntityId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEntityType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnKeyUse = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSubject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupSelectedCertificates = new System.Windows.Forms.GroupBox();
            this.buttonBottom = new System.Windows.Forms.Button();
            this.buttonTop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.groupAvailableCertificates = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.roleTypeFilter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ctfPath = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.CryptographicTrustFabricFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.certificateDetails = new System.Windows.Forms.GroupBox();
            this.signatureAlgorithmTextBox = new System.Windows.Forms.TextBox();
            this.thumbprintTextBox = new System.Windows.Forms.TextBox();
            this.validTextBox = new System.Windows.Forms.TextBox();
            this.serialNumberTextBox = new System.Windows.Forms.TextBox();
            this.issuerTextBox = new System.Windows.Forms.TextBox();
            this.signatureAlgorithmLabel = new System.Windows.Forms.Label();
            this.subjectTextBox = new System.Windows.Forms.TextBox();
            this.thumbprintLabel = new System.Windows.Forms.Label();
            this.validLabel = new System.Windows.Forms.Label();
            this.serialNumberLabel = new System.Windows.Forms.Label();
            this.issuerLabel = new System.Windows.Forms.Label();
            this.subjectLabel = new System.Windows.Forms.Label();
            this.InstallTo.SuspendLayout();
            this.groupSelectedCertificates.SuspendLayout();
            this.groupAvailableCertificates.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.certificateDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonInstall
            // 
            this.buttonInstall.Location = new System.Drawing.Point(523, 191);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(104, 23);
            this.buttonInstall.TabIndex = 3;
            this.buttonInstall.Text = "&Install";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // buttonUninstall
            // 
            this.buttonUninstall.Location = new System.Drawing.Point(523, 249);
            this.buttonUninstall.Name = "buttonUninstall";
            this.buttonUninstall.Size = new System.Drawing.Size(104, 23);
            this.buttonUninstall.TabIndex = 4;
            this.buttonUninstall.Text = "&Uninstall";
            this.buttonUninstall.UseVisualStyleBackColor = true;
            this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
            // 
            // buttonUninstallAll
            // 
            this.buttonUninstallAll.Location = new System.Drawing.Point(523, 278);
            this.buttonUninstallAll.Name = "buttonUninstallAll";
            this.buttonUninstallAll.Size = new System.Drawing.Size(104, 23);
            this.buttonUninstallAll.TabIndex = 5;
            this.buttonUninstallAll.Text = "Un&install All";
            this.buttonUninstallAll.UseVisualStyleBackColor = true;
            this.buttonUninstallAll.Click += new System.EventHandler(this.buttonUninstallAll_Click);
            // 
            // InstallTo
            // 
            this.InstallTo.Controls.Add(this.installToLocalComputerRadioButton);
            this.InstallTo.Controls.Add(this.installToUserRadioButton);
            this.InstallTo.Location = new System.Drawing.Point(523, 12);
            this.InstallTo.Name = "InstallTo";
            this.InstallTo.Size = new System.Drawing.Size(108, 56);
            this.InstallTo.TabIndex = 13;
            this.InstallTo.TabStop = false;
            this.InstallTo.Text = "Install To";
            this.InstallTo.Enter += new System.EventHandler(this.Runtime_Enter);
            // 
            // installToLocalComputerRadioButton
            // 
            this.installToLocalComputerRadioButton.AutoSize = true;
            this.installToLocalComputerRadioButton.Location = new System.Drawing.Point(5, 39);
            this.installToLocalComputerRadioButton.Name = "installToLocalComputerRadioButton";
            this.installToLocalComputerRadioButton.Size = new System.Drawing.Size(99, 17);
            this.installToLocalComputerRadioButton.TabIndex = 11;
            this.installToLocalComputerRadioButton.TabStop = true;
            this.installToLocalComputerRadioButton.Text = "Local Computer";
            this.installToLocalComputerRadioButton.UseVisualStyleBackColor = true;
            this.installToLocalComputerRadioButton.CheckedChanged += new System.EventHandler(this.installToLocalComputerRadioButton_CheckedChanged);
            // 
            // installToUserRadioButton
            // 
            this.installToUserRadioButton.AutoSize = true;
            this.installToUserRadioButton.Location = new System.Drawing.Point(4, 16);
            this.installToUserRadioButton.Name = "installToUserRadioButton";
            this.installToUserRadioButton.Size = new System.Drawing.Size(47, 17);
            this.installToUserRadioButton.TabIndex = 11;
            this.installToUserRadioButton.TabStop = true;
            this.installToUserRadioButton.Text = "User";
            this.installToUserRadioButton.UseVisualStyleBackColor = true;
            this.installToUserRadioButton.CheckedChanged += new System.EventHandler(this.installToUserRadioButton_CheckedChanged);
            // 
            // buttonInstallAll
            // 
            this.buttonInstallAll.Location = new System.Drawing.Point(523, 220);
            this.buttonInstallAll.Name = "buttonInstallAll";
            this.buttonInstallAll.Size = new System.Drawing.Size(104, 23);
            this.buttonInstallAll.TabIndex = 12;
            this.buttonInstallAll.Text = "Install &All";
            this.buttonInstallAll.UseVisualStyleBackColor = true;
            this.buttonInstallAll.Click += new System.EventHandler(this.buttonInstallAll_Click);
            // 
            // listInstalledCertificates
            // 
            this.listInstalledCertificates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEntityIdSelected,
            this.columnEntityTypeSelected,
            this.columnKeyUseSelected,
            this.columnSubjectSelected});
            this.listInstalledCertificates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listInstalledCertificates.FullRowSelect = true;
            this.listInstalledCertificates.HideSelection = false;
            this.listInstalledCertificates.Location = new System.Drawing.Point(642, 74);
            this.listInstalledCertificates.Name = "listInstalledCertificates";
            this.tableLayoutPanel1.SetRowSpan(this.listInstalledCertificates, 8);
            this.listInstalledCertificates.Size = new System.Drawing.Size(510, 440);
            this.listInstalledCertificates.TabIndex = 1;
            this.listInstalledCertificates.UseCompatibleStateImageBehavior = false;
            this.listInstalledCertificates.View = System.Windows.Forms.View.Details;
            this.listInstalledCertificates.SelectedIndexChanged += new System.EventHandler(this.listInstalledCertificates_SelectedIndexChanged);
            this.listInstalledCertificates.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listSelectedCertificates_MouseDoubleClick);
            // 
            // columnEntityIdSelected
            // 
            this.columnEntityIdSelected.Text = "Entity ID";
            this.columnEntityIdSelected.Width = 72;
            // 
            // columnEntityTypeSelected
            // 
            this.columnEntityTypeSelected.Text = "Entity Type";
            this.columnEntityTypeSelected.Width = 77;
            // 
            // columnKeyUseSelected
            // 
            this.columnKeyUseSelected.Text = "Key Use";
            this.columnKeyUseSelected.Width = 66;
            // 
            // columnSubjectSelected
            // 
            this.columnSubjectSelected.Text = "Subject";
            this.columnSubjectSelected.Width = 181;
            // 
            // listAvailableCertificates
            // 
            this.listAvailableCertificates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEntityId,
            this.columnEntityType,
            this.columnKeyUse,
            this.columnSubject});
            this.listAvailableCertificates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listAvailableCertificates.FullRowSelect = true;
            this.listAvailableCertificates.GridLines = true;
            this.listAvailableCertificates.HideSelection = false;
            this.listAvailableCertificates.Location = new System.Drawing.Point(3, 74);
            this.listAvailableCertificates.Name = "listAvailableCertificates";
            this.tableLayoutPanel1.SetRowSpan(this.listAvailableCertificates, 8);
            this.listAvailableCertificates.Size = new System.Drawing.Size(509, 440);
            this.listAvailableCertificates.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listAvailableCertificates.TabIndex = 0;
            this.listAvailableCertificates.UseCompatibleStateImageBehavior = false;
            this.listAvailableCertificates.View = System.Windows.Forms.View.Details;
            this.listAvailableCertificates.SelectedIndexChanged += new System.EventHandler(this.listAvailableCertificates_SelectedIndexChanged);
            this.listAvailableCertificates.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listAvailableCertificates_MouseDoubleClick);
            // 
            // columnEntityId
            // 
            this.columnEntityId.Text = "Entity ID";
            this.columnEntityId.Width = 67;
            // 
            // columnEntityType
            // 
            this.columnEntityType.Text = "Entity Type";
            this.columnEntityType.Width = 72;
            // 
            // columnKeyUse
            // 
            this.columnKeyUse.Text = "Key Use";
            this.columnKeyUse.Width = 70;
            // 
            // columnSubject
            // 
            this.columnSubject.Text = "Subject";
            this.columnSubject.Width = 187;
            // 
            // groupSelectedCertificates
            // 
            this.groupSelectedCertificates.Controls.Add(this.buttonBottom);
            this.groupSelectedCertificates.Controls.Add(this.buttonTop);
            this.groupSelectedCertificates.Controls.Add(this.label1);
            this.groupSelectedCertificates.Controls.Add(this.buttonDown);
            this.groupSelectedCertificates.Controls.Add(this.buttonUp);
            this.groupSelectedCertificates.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupSelectedCertificates.Location = new System.Drawing.Point(642, 12);
            this.groupSelectedCertificates.Name = "groupSelectedCertificates";
            this.groupSelectedCertificates.Size = new System.Drawing.Size(510, 44);
            this.groupSelectedCertificates.TabIndex = 2;
            this.groupSelectedCertificates.TabStop = false;
            // 
            // buttonBottom
            // 
            this.buttonBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBottom.Image = ((System.Drawing.Image)(resources.GetObject("buttonBottom.Image")));
            this.buttonBottom.Location = new System.Drawing.Point(399, 13);
            this.buttonBottom.Name = "buttonBottom";
            this.buttonBottom.Size = new System.Drawing.Size(31, 23);
            this.buttonBottom.TabIndex = 1;
            this.buttonBottom.UseVisualStyleBackColor = true;
            this.buttonBottom.Click += new System.EventHandler(this.buttonBottom_Click);
            // 
            // buttonTop
            // 
            this.buttonTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTop.Image = ((System.Drawing.Image)(resources.GetObject("buttonTop.Image")));
            this.buttonTop.Location = new System.Drawing.Point(362, 13);
            this.buttonTop.Name = "buttonTop";
            this.buttonTop.Size = new System.Drawing.Size(31, 23);
            this.buttonTop.TabIndex = 0;
            this.buttonTop.UseVisualStyleBackColor = true;
            this.buttonTop.Click += new System.EventHandler(this.buttonTop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Installed Certificates";
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonDown.Image")));
            this.buttonDown.Location = new System.Drawing.Point(473, 13);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(31, 23);
            this.buttonDown.TabIndex = 3;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonUp.Image")));
            this.buttonUp.Location = new System.Drawing.Point(436, 13);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(31, 23);
            this.buttonUp.TabIndex = 2;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // groupAvailableCertificates
            // 
            this.groupAvailableCertificates.Controls.Add(this.label3);
            this.groupAvailableCertificates.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupAvailableCertificates.Location = new System.Drawing.Point(3, 12);
            this.groupAvailableCertificates.Name = "groupAvailableCertificates";
            this.groupAvailableCertificates.Size = new System.Drawing.Size(509, 44);
            this.groupAvailableCertificates.TabIndex = 10;
            this.groupAvailableCertificates.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Available Certificates";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.listAvailableCertificates, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.listInstalledCertificates, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupAvailableCertificates, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupSelectedCertificates, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.InstallTo, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonUninstall, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.buttonInstallAll, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.buttonInstall, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.buttonUninstallAll, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 2, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 38);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1155, 517);
            this.tableLayoutPanel1.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.roleTypeFilter);
            this.groupBox1.Location = new System.Drawing.Point(523, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(108, 40);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RoleType Filter";
            // 
            // roleTypeFilter
            // 
            this.roleTypeFilter.FormattingEnabled = true;
            this.roleTypeFilter.Location = new System.Drawing.Point(4, 13);
            this.roleTypeFilter.Name = "roleTypeFilter";
            this.roleTypeFilter.Size = new System.Drawing.Size(98, 21);
            this.roleTypeFilter.TabIndex = 14;
            this.roleTypeFilter.SelectedIndexChanged += new System.EventHandler(this.roleTypeFilter_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "CTF file location:";
            // 
            // ctfPath
            // 
            this.ctfPath.AcceptsReturn = true;
            this.ctfPath.Location = new System.Drawing.Point(100, 9);
            this.ctfPath.Name = "ctfPath";
            this.ctfPath.Size = new System.Drawing.Size(423, 20);
            this.ctfPath.TabIndex = 15;
            this.ctfPath.TextChanged += new System.EventHandler(this.ctfPath_TextChanged);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(534, 9);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(104, 23);
            this.browseButton.TabIndex = 16;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // CryptographicTrustFabricFileDialog
            // 
            this.CryptographicTrustFabricFileDialog.Filter = "CTF file|*.xml";
            this.CryptographicTrustFabricFileDialog.Title = "Select  the Cryptographic Trust Fabric file";
            // 
            // certificateDetails
            // 
            this.certificateDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.certificateDetails.Controls.Add(this.signatureAlgorithmTextBox);
            this.certificateDetails.Controls.Add(this.thumbprintTextBox);
            this.certificateDetails.Controls.Add(this.validTextBox);
            this.certificateDetails.Controls.Add(this.serialNumberTextBox);
            this.certificateDetails.Controls.Add(this.issuerTextBox);
            this.certificateDetails.Controls.Add(this.signatureAlgorithmLabel);
            this.certificateDetails.Controls.Add(this.subjectTextBox);
            this.certificateDetails.Controls.Add(this.thumbprintLabel);
            this.certificateDetails.Controls.Add(this.validLabel);
            this.certificateDetails.Controls.Add(this.serialNumberLabel);
            this.certificateDetails.Controls.Add(this.issuerLabel);
            this.certificateDetails.Controls.Add(this.subjectLabel);
            this.certificateDetails.Location = new System.Drawing.Point(11, 567);
            this.certificateDetails.Name = "certificateDetails";
            this.certificateDetails.Size = new System.Drawing.Size(1155, 100);
            this.certificateDetails.TabIndex = 17;
            this.certificateDetails.TabStop = false;
            this.certificateDetails.Text = "Certificate Details";
            // 
            // signatureAlgorithmTextBox
            // 
            this.signatureAlgorithmTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.signatureAlgorithmTextBox.Location = new System.Drawing.Point(695, 73);
            this.signatureAlgorithmTextBox.Name = "signatureAlgorithmTextBox";
            this.signatureAlgorithmTextBox.ReadOnly = true;
            this.signatureAlgorithmTextBox.Size = new System.Drawing.Size(457, 20);
            this.signatureAlgorithmTextBox.TabIndex = 2;
            // 
            // thumbprintTextBox
            // 
            this.thumbprintTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.thumbprintTextBox.Location = new System.Drawing.Point(695, 45);
            this.thumbprintTextBox.Name = "thumbprintTextBox";
            this.thumbprintTextBox.ReadOnly = true;
            this.thumbprintTextBox.Size = new System.Drawing.Size(457, 20);
            this.thumbprintTextBox.TabIndex = 2;
            // 
            // validTextBox
            // 
            this.validTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.validTextBox.Location = new System.Drawing.Point(55, 70);
            this.validTextBox.Name = "validTextBox";
            this.validTextBox.ReadOnly = true;
            this.validTextBox.Size = new System.Drawing.Size(457, 20);
            this.validTextBox.TabIndex = 2;
            // 
            // serialNumberTextBox
            // 
            this.serialNumberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.serialNumberTextBox.Location = new System.Drawing.Point(695, 16);
            this.serialNumberTextBox.Name = "serialNumberTextBox";
            this.serialNumberTextBox.ReadOnly = true;
            this.serialNumberTextBox.Size = new System.Drawing.Size(457, 20);
            this.serialNumberTextBox.TabIndex = 2;
            // 
            // issuerTextBox
            // 
            this.issuerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.issuerTextBox.Location = new System.Drawing.Point(55, 42);
            this.issuerTextBox.Name = "issuerTextBox";
            this.issuerTextBox.ReadOnly = true;
            this.issuerTextBox.Size = new System.Drawing.Size(457, 20);
            this.issuerTextBox.TabIndex = 2;
            // 
            // signatureAlgorithmLabel
            // 
            this.signatureAlgorithmLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.signatureAlgorithmLabel.AutoSize = true;
            this.signatureAlgorithmLabel.Location = new System.Drawing.Point(591, 76);
            this.signatureAlgorithmLabel.Name = "signatureAlgorithmLabel";
            this.signatureAlgorithmLabel.Size = new System.Drawing.Size(101, 13);
            this.signatureAlgorithmLabel.TabIndex = 1;
            this.signatureAlgorithmLabel.Text = "Signature Algorithm:";
            this.signatureAlgorithmLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // subjectTextBox
            // 
            this.subjectTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.subjectTextBox.Location = new System.Drawing.Point(55, 16);
            this.subjectTextBox.Name = "subjectTextBox";
            this.subjectTextBox.ReadOnly = true;
            this.subjectTextBox.Size = new System.Drawing.Size(457, 20);
            this.subjectTextBox.TabIndex = 2;
            // 
            // thumbprintLabel
            // 
            this.thumbprintLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.thumbprintLabel.AutoSize = true;
            this.thumbprintLabel.Location = new System.Drawing.Point(629, 48);
            this.thumbprintLabel.Name = "thumbprintLabel";
            this.thumbprintLabel.Size = new System.Drawing.Size(63, 13);
            this.thumbprintLabel.TabIndex = 0;
            this.thumbprintLabel.Text = "Thumbprint:";
            this.thumbprintLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // validLabel
            // 
            this.validLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.validLabel.AutoSize = true;
            this.validLabel.Location = new System.Drawing.Point(16, 70);
            this.validLabel.Name = "validLabel";
            this.validLabel.Size = new System.Drawing.Size(33, 13);
            this.validLabel.TabIndex = 1;
            this.validLabel.Text = "Valid:";
            this.validLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // serialNumberLabel
            // 
            this.serialNumberLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.serialNumberLabel.AutoSize = true;
            this.serialNumberLabel.Location = new System.Drawing.Point(616, 16);
            this.serialNumberLabel.Name = "serialNumberLabel";
            this.serialNumberLabel.Size = new System.Drawing.Size(76, 13);
            this.serialNumberLabel.TabIndex = 0;
            this.serialNumberLabel.Text = "Serial Number:";
            this.serialNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // issuerLabel
            // 
            this.issuerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.issuerLabel.AutoSize = true;
            this.issuerLabel.Location = new System.Drawing.Point(11, 42);
            this.issuerLabel.Name = "issuerLabel";
            this.issuerLabel.Size = new System.Drawing.Size(38, 13);
            this.issuerLabel.TabIndex = 0;
            this.issuerLabel.Text = "Issuer:";
            this.issuerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // subjectLabel
            // 
            this.subjectLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.subjectLabel.AutoSize = true;
            this.subjectLabel.Location = new System.Drawing.Point(3, 16);
            this.subjectLabel.Name = "subjectLabel";
            this.subjectLabel.Size = new System.Drawing.Size(46, 13);
            this.subjectLabel.TabIndex = 0;
            this.subjectLabel.Text = "Subject:";
            this.subjectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CertificatesMgrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 679);
            this.Controls.Add(this.certificateDetails);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.ctfPath);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1194, 715);
            this.Name = "CertificatesMgrForm";
            this.Text = "Cryptographic Trust Fabric Manager";
            this.Load += new System.EventHandler(this.CertificatesMgrForm_Load);
            this.InstallTo.ResumeLayout(false);
            this.InstallTo.PerformLayout();
            this.groupSelectedCertificates.ResumeLayout(false);
            this.groupSelectedCertificates.PerformLayout();
            this.groupAvailableCertificates.ResumeLayout(false);
            this.groupAvailableCertificates.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.certificateDetails.ResumeLayout(false);
            this.certificateDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button buttonInstall;
      private System.Windows.Forms.Button buttonUninstall;
      private System.Windows.Forms.Button buttonUninstallAll;
      private System.Windows.Forms.GroupBox InstallTo;
      private System.Windows.Forms.RadioButton installToLocalComputerRadioButton;
      private System.Windows.Forms.RadioButton installToUserRadioButton;
      private System.Windows.Forms.Button buttonInstallAll;
      private System.Windows.Forms.ListView listInstalledCertificates;
      private System.Windows.Forms.ColumnHeader columnEntityIdSelected;
      private System.Windows.Forms.ColumnHeader columnEntityTypeSelected;
      private System.Windows.Forms.ColumnHeader columnKeyUseSelected;
      private System.Windows.Forms.ColumnHeader columnSubjectSelected;
      private System.Windows.Forms.ListView listAvailableCertificates;
      private System.Windows.Forms.ColumnHeader columnEntityId;
      private System.Windows.Forms.ColumnHeader columnEntityType;
      private System.Windows.Forms.ColumnHeader columnKeyUse;
      private System.Windows.Forms.ColumnHeader columnSubject;
      private System.Windows.Forms.GroupBox groupSelectedCertificates;
      private System.Windows.Forms.Button buttonBottom;
      private System.Windows.Forms.Button buttonTop;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button buttonDown;
      private System.Windows.Forms.Button buttonUp;
      private System.Windows.Forms.GroupBox groupAvailableCertificates;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox ctfPath;
      private System.Windows.Forms.Button browseButton;
      private System.Windows.Forms.OpenFileDialog CryptographicTrustFabricFileDialog;
      private System.Windows.Forms.ComboBox roleTypeFilter;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox certificateDetails;
      private System.Windows.Forms.Label subjectLabel;
      private System.Windows.Forms.Label validLabel;
      private System.Windows.Forms.Label issuerLabel;
      private System.Windows.Forms.TextBox signatureAlgorithmTextBox;
      private System.Windows.Forms.TextBox thumbprintTextBox;
      private System.Windows.Forms.TextBox validTextBox;
      private System.Windows.Forms.TextBox serialNumberTextBox;
      private System.Windows.Forms.TextBox issuerTextBox;
      private System.Windows.Forms.Label signatureAlgorithmLabel;
      private System.Windows.Forms.TextBox subjectTextBox;
      private System.Windows.Forms.Label thumbprintLabel;
      private System.Windows.Forms.Label serialNumberLabel;
   }
}
