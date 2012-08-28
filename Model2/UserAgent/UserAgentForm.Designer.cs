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

namespace CommercialVehicleCollisionClient
{
    public partial class UserAgentForm
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.ResultListBox = new System.Windows.Forms.ListBox();
            this.GetDocument = new System.Windows.Forms.Button();
            this.upLoadPhoto = new System.Windows.Forms.Button();
            this.downloadData = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ResultListBox
            // 
            this.ResultListBox.FormattingEnabled = true;
            this.ResultListBox.Location = new System.Drawing.Point(6, 16);
            this.ResultListBox.Name = "ResultListBox";
            this.ResultListBox.Size = new System.Drawing.Size(273, 95);
            this.ResultListBox.TabIndex = 6;
            this.ResultListBox.Visible = false;
            // 
            // GetDocument
            // 
            this.GetDocument.Location = new System.Drawing.Point(12, 13);
            this.GetDocument.Name = "GetDocument";
            this.GetDocument.Size = new System.Drawing.Size(75, 23);
            this.GetDocument.TabIndex = 7;
            this.GetDocument.Text = "Get Doc";
            this.GetDocument.UseVisualStyleBackColor = true;
            this.GetDocument.Click += new System.EventHandler(this.GetDocument_Click);
            // 
            // upLoadPhoto
            // 
            this.upLoadPhoto.Location = new System.Drawing.Point(117, 13);
            this.upLoadPhoto.Name = "upLoadPhoto";
            this.upLoadPhoto.Size = new System.Drawing.Size(75, 23);
            this.upLoadPhoto.TabIndex = 8;
            this.upLoadPhoto.Text = "Upload Photo";
            this.upLoadPhoto.UseVisualStyleBackColor = true;
            this.upLoadPhoto.Click += new System.EventHandler(this.upLoadPhoto_Click);
            // 
            // downloadData
            // 
            this.downloadData.Location = new System.Drawing.Point(222, 13);
            this.downloadData.Name = "downloadData";
            this.downloadData.Size = new System.Drawing.Size(75, 23);
            this.downloadData.TabIndex = 9;
            this.downloadData.Text = "Download Data";
            this.downloadData.UseVisualStyleBackColor = true;
            this.downloadData.Click += new System.EventHandler(this.downloadData_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ResultListBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 114);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Results:";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // UserAgentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 188);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.downloadData);
            this.Controls.Add(this.upLoadPhoto);
            this.Controls.Add(this.GetDocument);
            this.Name = "UserAgentForm";
            this.Text = "User Agent";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ResultListBox;
        private System.Windows.Forms.Button GetDocument;
        private System.Windows.Forms.Button upLoadPhoto;
        private System.Windows.Forms.Button downloadData;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

