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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Net;

using GfipmCryptoTrustFabric;
using GfipmCryptoTrustFabricModel;

namespace CryptoTrustFabricMgr
{
    public partial class CertificatesMgrForm : Form
    {
        private EntitiesDescriptorType _entitiesDescriptor = null;

        public CertificatesMgrForm()
        {
            InitializeComponent();

            listAvailableCertificates.MouseDown += new System.Windows.Forms.MouseEventHandler(listAvailableCertificates_MouseDown);
            listInstalledCertificates.MouseDown += new System.Windows.Forms.MouseEventHandler(listInstalledCertificates_MouseDown);

            ctfPath.KeyDown += new System.Windows.Forms.KeyEventHandler(ctfPath_KeyDown);


            // default to install to LocalComputer
            this.installToLocalComputerRadioButton.Select();

            roleTypeFilter.Items.Add("Display All");
            roleTypeFilter.Items.Add("ApplicationService");
            roleTypeFilter.Items.Add("AttributeAuthority"); 
            roleTypeFilter.Items.Add("AttributeService");
            roleTypeFilter.Items.Add("AuthnAuthority"); 
            roleTypeFilter.Items.Add("PDP"); 
            roleTypeFilter.Items.Add("PseudonymService"); 
            roleTypeFilter.Items.Add("SecurityTokenService");
            roleTypeFilter.Items.Add("IDPSSO");
            roleTypeFilter.Items.Add("SSO");
            roleTypeFilter.Items.Add("SPSSO");
            roleTypeFilter.Items.Add("WSP"); 
            roleTypeFilter.Items.Add("WSC");
            roleTypeFilter.Items.Add("ADS");
            
            roleTypeFilter.SelectedIndex = 0;
        }
                
        #region Private Methods

        private ListViewItem CreateListViewItem(CtfCertificate ctfCertificate, Color color)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = ctfCertificate.EntityId;
            lvi.ForeColor = color;

            lvi.SubItems.Add(ctfCertificate.Type);
            lvi.SubItems.Add(ctfCertificate.KeyUse);
            lvi.SubItems.Add(ctfCertificate.Cert.SubjectName.Name);
            lvi.Tag = ctfCertificate;

            return lvi;
        }

        private void AddAvailableCertificateItem(CtfCertificate ctfCertificate)
        {
            ListViewItem lvi = CreateListViewItem(ctfCertificate, Color.Black);

            listAvailableCertificates.Items.Add(lvi);
        }

        private void AddInstalledCertificateItem(CtfCertificate ctfCertificate)
        {
            ListViewItem lvi = null;

            if (!string.IsNullOrEmpty(ctfCertificate.EntityId))
            {
                lvi = CreateListViewItem(ctfCertificate, Color.Black);
            }
            else
            {
                lvi = CreateListViewItem(ctfCertificate, Color.Red);
            }

            listInstalledCertificates.Items.Add(lvi);
        }

        private void SetButtons()
        {
            buttonInstall.Enabled = (listAvailableCertificates.SelectedItems.Count > 0);
            buttonUninstall.Enabled = (listInstalledCertificates.SelectedItems.Count > 0);
            buttonInstallAll.Enabled = (listAvailableCertificates.Items.Count > 0);
            buttonUninstallAll.Enabled = (listInstalledCertificates.Items.Count > 0);
        }

        private void listAvailableCertificates_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listAvailableCertificates.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                ClearCertificateDetails();

                UpdateCertificateDetails(item.Tag as CtfCertificate);

                ShowCertificateGui(item.Tag as CtfCertificate);
            }
        }

        private void listSelectedCertificates_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listInstalledCertificates.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                ClearCertificateDetails();

                UpdateCertificateDetails(item.Tag as CtfCertificate);

                ShowCertificateGui(item.Tag as CtfCertificate);
            }
        }

        private string MapEntityTypeToString(Type type)
        {
            string roleType = string.Empty;

            switch (type.Name)
            {
                case "ApplicationServiceType":
                    roleType = "ApplicationService";
                    break;
                case "AttributeServiceType":
                    roleType = "AttributeService";
                    break;
                case "PseudonymServiceType":
                    roleType = "PseudonymService";
                    break;
                case "SecurityTokenServiceType":
                    roleType = "SecurityTokenService";
                    break;
                case "AttributeAuthorityDescriptorType":
                    roleType = "AttributeAuthority";
                    break;
                case "PDPDescriptorType":
                    roleType = "PDP";
                    break;
                case "AuthnAuthorityDescriptorType":
                    roleType = "AuthnAuthority";
                    break;
                case "SSODescriptorType":
                    roleType = "SSO";
                    break;
                case "SPSSODescriptorType":
                    roleType = "SPSSO";
                    break;
                case "IDPSSODescriptorType":
                    roleType = "IDPSSO";
                    break;
                case "GFIPMWebServiceConsumerType":
                    roleType = "WSC";
                    break;
                case "GFIPMAssertionDelegateServiceType":
                    roleType = "ADS";
                    break;
                case "GFIPMWebServiceProviderType":
                    roleType = "WSP";
                    break;

                default:
                    throw new InvalidOperationException("roleType");
            }
            return roleType;
        }

        private string RoleTypeFromFilter(string roleTypeFilter)
        {
            string roleType = string.Empty;

            switch (roleTypeFilter)
            {
                case "ApplicationService":
                    roleType = "ApplicationServiceType";
                    break;
                case "AttributeService":
                    roleType = "AttributeServiceType";
                    break;
                case "PseudonymService":
                    roleType = "PseudonymServiceType";
                    break;
                case "SecurityTokenService":
                    roleType = "SecurityTokenServiceType";
                    break;
                case "AttributeAuthority":
                    roleType = "AttributeAuthorityDescriptorType";
                    break;
                case "PDP":
                    roleType = "PDPDescriptorType";
                    break;
                case "AuthnAuthority":
                    roleType = "AuthnAuthorityDescriptorType";
                    break;
                case "SSO":
                    roleType = "SSODescriptorType";
                    break;
                case "SPSSO":
                    roleType = "SPSSODescriptorType";
                    break;
                case "IDPSSO":
                    roleType = "IDPSSODescriptorType";
                    break;
                case "WSC":
                    roleType = "GFIPMWebServiceConsumerType";
                    break;
                case "ADS":
                    roleType = "GFIPMAssertionDelegateServiceType";
                    break;
                case "WSP":
                    roleType = "GFIPMWebServiceProviderType";
                    break;
                default:
                    break;
            }
            return roleType;
        }

        private List<CtfCertificate> GetAllCtfCertificates(string roleTypeFilter)
        {
            string roleType = RoleTypeFromFilter(roleTypeFilter);

            List<CtfCertificate> certs = new List<CtfCertificate>();

            EntitiesDescriptorType entitiesDescriptor = _entitiesDescriptor;

            bool displayAll = (roleTypeFilter == "Display All");

            foreach (object o in entitiesDescriptor.Items)
            {
                EntityDescriptorType ed = o as EntityDescriptorType;
                if (ed != null)
                {
                    foreach (object obj in ed.Items)
                    {
                        Type type = obj.GetType();

                        string entityType = MapEntityTypeToString(type);

                        RoleDescriptorType rd = obj as RoleDescriptorType;

                        if (rd != null && (displayAll || type.Name == roleType))
                        {
                            foreach (KeyDescriptorType keyDesc in rd.KeyDescriptor)
                            {
                                foreach (object ob in keyDesc.KeyInfo.Items)
                                {
                                    if (ob != null)
                                    {
                                        X509DataType x509 = ob as X509DataType;
                                        if (x509 != null)
                                        {
                                            foreach (object o2 in x509.Items)
                                            {
                                                if (o2 != null)
                                                {
                                                    Type certType = o2.GetType();

                                                    if (certType == typeof(byte[]))
                                                    {
                                                        byte[] bytes = o2 as byte[];

                                                        X509Certificate2 x509Cert = new X509Certificate2(bytes);

                                                        CtfCertificate ctfCert = new CtfCertificate(ed.entityID, keyDesc.use.ToString(), entityType, x509Cert);
                                                        certs.Add(ctfCert);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return certs;
        }

        X509Certificate2Collection ConvertToCertificatesCollection(List<CtfCertificate> ctfCerts)
        {
            X509Certificate2Collection certs = new X509Certificate2Collection();

            foreach (CtfCertificate cert in ctfCerts)
            {
                certs.Add(cert.Cert);
            }

            return certs;
        }

        public Dictionary<string, X509Certificate2> GetCurrentStoreCertificates()
        {
            Dictionary<string, X509Certificate2> certs = new Dictionary<string, X509Certificate2>();

            X509Store x509Store = null;
            try
            {
                StoreLocation storeLocation = StoreLocation.LocalMachine;

                if (this.installToUserRadioButton.Checked)
                {
                    storeLocation = StoreLocation.CurrentUser;
                }

                x509Store = new X509Store(StoreName.TrustedPeople, storeLocation);

                x509Store.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);

                foreach (X509Certificate2 x509 in x509Store.Certificates)
                {
                    certs[x509.Thumbprint] = x509;
                }

                x509Store.Close();

                x509Store = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                if (x509Store != null)
                {
                    x509Store.Close();
                }
            }

            return certs;
        }

        public void StoreEntityCertificates(List<CtfCertificate> ctfCerts)
        {
            X509Certificate2Collection certs = ConvertToCertificatesCollection(ctfCerts);

            StoreLocation storeLocation = StoreLocation.LocalMachine;

            if (this.installToUserRadioButton.Checked)
            {
                storeLocation = StoreLocation.CurrentUser;
            }

            StoreEntityCertificatesCore(certs, StoreName.TrustedPeople, storeLocation);
            
            // now select self-signed certificates
            X509Certificate2Collection selfSignedCerts = GetSelfSignedCertificates(certs);

            StoreEntityCertificatesCore(certs, StoreName.AuthRoot, storeLocation);
        }

        private X509Certificate2Collection GetSelfSignedCertificates(X509Certificate2Collection certs)
        {
            X509Certificate2Collection selfSignedCerts = new X509Certificate2Collection();

            foreach (X509Certificate2 cert in certs)
            {
                if (cert.Subject == cert.Issuer)
                {
                    selfSignedCerts.Add(cert);
                }
            }

            return selfSignedCerts;
        }

        private void StoreEntityCertificatesCore(X509Certificate2Collection certs, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store x509Store = null;

            x509Store = new X509Store(storeName, storeLocation);

            x509Store.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);

            x509Store.AddRange(certs);

            x509Store.Close();
        }

        public void RemoveEntityCertificates(List<CtfCertificate> ctfCerts)
        {
            X509Certificate2Collection certs = ConvertToCertificatesCollection(ctfCerts);

            StoreLocation storeLocation = StoreLocation.LocalMachine;

            if (this.installToUserRadioButton.Checked)
            {
                storeLocation = StoreLocation.CurrentUser;
            }

            RemoveEntityCertificatesCore(certs, StoreName.TrustedPeople, storeLocation);

            // now select self-signed certificates
            X509Certificate2Collection selfSignedCerts = GetSelfSignedCertificates(certs);

            RemoveEntityCertificatesCore(certs, StoreName.AuthRoot, storeLocation);
        }

        private void RemoveEntityCertificatesCore(X509Certificate2Collection certs, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store x509Store = null;

            x509Store = new X509Store(storeName, storeLocation);

            x509Store.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);

            x509Store.RemoveRange(certs);

            x509Store.Close();
        }

        public void ShowCertificateGui(CtfCertificate cert)
        {
            X509Certificate2UI.DisplayCertificate(cert.Cert, this.Handle);
        }

        private void Reload()
        {
            if (_entitiesDescriptor == null)
            {
                return;
            }

            ClearCertificateDetails();

            listAvailableCertificates.Items.Clear();
            listInstalledCertificates.Items.Clear();

            string roleFilter = roleTypeFilter.Items[roleTypeFilter.SelectedIndex] as string;

            List<CtfCertificate> certs = this.GetAllCtfCertificates(roleFilter);

            Dictionary<string, X509Certificate2> currentStoreCertificates = GetCurrentStoreCertificates();

            // Add each of the available certificates.
            foreach (CtfCertificate ctfCert in certs)
            {
                if (currentStoreCertificates.ContainsKey(ctfCert.Cert.Thumbprint))
                {
                    AddInstalledCertificateItem(ctfCert);
                }
                else
                {
                    AddAvailableCertificateItem(ctfCert);
                }
            }

            SetButtons();
        }

        private void UpdateCertificateDetails(CtfCertificate cert)
        {
            this.subjectTextBox.Text = cert.Cert.Subject;

            this.issuerTextBox.Text = cert.Cert.Issuer;

            this.validTextBox.Text = "From <" + cert.Cert.GetEffectiveDateString() + "> To <" + cert.Cert.GetExpirationDateString() + ">";

            this.serialNumberTextBox.Text = cert.Cert.SerialNumber;

            this.thumbprintTextBox.Text = cert.Cert.Thumbprint;

            this.signatureAlgorithmTextBox.Text = cert.Cert.SignatureAlgorithm.FriendlyName;
        }

        private void ClearCertificateDetails()
        {
            this.subjectTextBox.Clear();
            this.issuerTextBox.Clear();
            this.validTextBox.Clear();
            this.serialNumberTextBox.Clear();
            this.thumbprintTextBox.Clear();
            this.signatureAlgorithmTextBox.Clear();
        }

        #endregion
        
        #region Form Handlers

        private void buttonTop_Click(object sender, EventArgs e)
        {
            // Move up each selected item to the top of the control.
            int index = 0;

            foreach (ListViewItem lvi in listInstalledCertificates.SelectedItems)
            {
                if (lvi.Index != index)
                {
                    listInstalledCertificates.Items.Remove(lvi);
                    listInstalledCertificates.Items.Insert(index, lvi);
                }
                index++;
            }
        }

        private void buttonBottom_Click(object sender, EventArgs e)
        {
            // Move up each selected item to the Bottom of the control.
            foreach (ListViewItem lvi in listInstalledCertificates.SelectedItems)
            {
                if (lvi.Index != listInstalledCertificates.Items.Count - 1)
                {
                    listInstalledCertificates.Items.Remove(lvi);
                    listInstalledCertificates.Items.Add(lvi);
                }
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            // Move each selected item up 1 in the control.
            int skipAtTop = 0;
            foreach (ListViewItem lvi in listInstalledCertificates.SelectedItems)
            {
                if (lvi.Index > skipAtTop)
                {
                    int index = lvi.Index;
                    listInstalledCertificates.Items.Remove(lvi);
                    listInstalledCertificates.Items.Insert(index - 1, lvi);
                }
                else
                {
                    skipAtTop++;
                }
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            SortedDictionary<int, ListViewItem> removed_items = new SortedDictionary<int, ListViewItem>();
            int count = 0;

            // Move each selected item down 1 in the control.
            foreach (ListViewItem lvi in listInstalledCertificates.SelectedItems)
            {
                int index = lvi.Index;
                listInstalledCertificates.Items.Remove(lvi);

                removed_items.Add(index + count++, lvi);
            }

            // Add each item down 1.
            foreach (KeyValuePair<int, ListViewItem> kvp in removed_items)
            {
                if (kvp.Key + 1 >= listInstalledCertificates.Items.Count)
                    listInstalledCertificates.Items.Add(kvp.Value);
                else
                    listInstalledCertificates.Items.Insert(kvp.Key + 1, kvp.Value);
            }
        }
                
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CertificatesMgrForm_Load(object sender, EventArgs e)
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;

            // Set up the ToolTip text for the CtfPath text box.
            toolTip.SetToolTip(this.ctfPath, "Type a valid url to download a CTF document then press ENTER.");
        }

        private void Runtime_Enter(object sender, EventArgs e)
        {

        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            List<CtfCertificate> ctfCerts = new List<CtfCertificate>();

            foreach (ListViewItem lvi in listAvailableCertificates.SelectedItems)
            {
                ctfCerts.Add(lvi.Tag as CtfCertificate);
            }

            StoreEntityCertificates(ctfCerts);

            Reload();
        }

        private void buttonInstallAll_Click(object sender, EventArgs e)
        {
            List<CtfCertificate> ctfCerts = new List<CtfCertificate>();

            foreach (ListViewItem lvi in listAvailableCertificates.Items)
            {
                ctfCerts.Add(lvi.Tag as CtfCertificate);
            }

            StoreEntityCertificates(ctfCerts);

            Reload();
        }

        private void buttonUninstall_Click(object sender, EventArgs e)
        {
            List<CtfCertificate> ctfCerts = new List<CtfCertificate>();

            foreach (ListViewItem lvi in listInstalledCertificates.SelectedItems)
            {
                ctfCerts.Add(lvi.Tag as CtfCertificate);
            }

            RemoveEntityCertificates(ctfCerts);

            Reload();
        }

        private void buttonUninstallAll_Click(object sender, EventArgs e)
        {
            List<CtfCertificate> ctfCerts = new List<CtfCertificate>();

            foreach (ListViewItem lvi in listInstalledCertificates.Items)
            {
                ctfCerts.Add(lvi.Tag as CtfCertificate);
            }

            RemoveEntityCertificates(ctfCerts);

            Reload();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (this.CryptographicTrustFabricFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ctfPath.Text = CryptographicTrustFabricFileDialog.FileName;

                GfipmCryptoTrustFabric.GfipmCryptoTrustFabric tf = new GfipmCryptoTrustFabric.GfipmCryptoTrustFabric();

                try
                {
                    _entitiesDescriptor = tf.OpenCtfFileAndValidateSignature(this.ctfPath.Text);
         
                    Reload();
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error: {0}. {1}",
                        ex.Message, (ex.InnerException != null) ? ex.InnerException.Message : "");
                    System.Windows.Forms.MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void roleTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Reload();
        }

        private void installToUserRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Reload();
        }

        private void installToLocalComputerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Reload();
        }

        private void listAvailableCertificates_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearCertificateDetails();

            ListView lv = sender as ListView;
            if (lv != null && lv.SelectedItems.Count > 0)
            {
                ListViewItem item = lv.SelectedItems[0];
                
                UpdateCertificateDetails(item.Tag as CtfCertificate);
            }

            SetButtons();
        }

        private void listInstalledCertificates_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearCertificateDetails();

            ListView lv = sender as ListView;
            if (lv != null && lv.SelectedItems.Count > 0)
            {
                ListViewItem item = lv.SelectedItems[0];

                UpdateCertificateDetails(item.Tag as CtfCertificate);
            }

            SetButtons();
        }

        private void listAvailableCertificates_MouseDown(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv != null && lv.SelectedItems.Count == 0 )
            {
                ClearCertificateDetails();
            }

            SetButtons();
        }

        private void listInstalledCertificates_MouseDown(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv != null && lv.SelectedItems.Count == 0 )
            {
                ClearCertificateDetails();
            }

            SetButtons();
        }       
        
        private void ctfPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string url = ctfPath.Text;

                try
                {
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.UserAgent = @"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";

                    using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.PreserveWhitespace = true;

                        xmlDocument.Load(httpResponse.GetResponseStream());

                        GfipmCryptoTrustFabric.GfipmCryptoTrustFabric tf = new GfipmCryptoTrustFabric.GfipmCryptoTrustFabric();

                        _entitiesDescriptor = tf.LoadFromXml(xmlDocument.OuterXml);

                        Reload();
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error: {0}. {1}",
                        ex.Message, (ex.InnerException != null) ? ex.InnerException.Message : "");
                    System.Windows.Forms.MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
 
        private void ctfPath_TextChanged(object sender, EventArgs e)
        {

        }
        
        #endregion
    }
}
