#pragma warning disable 1591

namespace Ascend.Infrastructure.TicketJones {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="xtics_rpcBinding", Namespace="urn:xtics_rpc")]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(performer))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(ticket))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(venue))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(ticketcategory))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(eventcategory))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(@event))]
    public partial class xtics_rpc : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback get_driftOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_event_countOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_eventsOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_event_category_countOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_event_categoriesOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_ticket_category_countOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_ticket_categoriesOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_venue_countOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_venuesOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_ticketsOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_ticket_infoOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_performer_countOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_performersOperationCompleted;
        
        private System.Threading.SendOrPostCallback purchase_orderOperationCompleted;
        
        private System.Threading.SendOrPostCallback get_events_by_zipcodeOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public xtics_rpc() {
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event get_driftCompletedEventHandler get_driftCompleted;
        
        /// <remarks/>
        public event get_event_countCompletedEventHandler get_event_countCompleted;
        
        /// <remarks/>
        public event get_eventsCompletedEventHandler get_eventsCompleted;
        
        /// <remarks/>
        public event get_event_category_countCompletedEventHandler get_event_category_countCompleted;
        
        /// <remarks/>
        public event get_event_categoriesCompletedEventHandler get_event_categoriesCompleted;
        
        /// <remarks/>
        public event get_ticket_category_countCompletedEventHandler get_ticket_category_countCompleted;
        
        /// <remarks/>
        public event get_ticket_categoriesCompletedEventHandler get_ticket_categoriesCompleted;
        
        /// <remarks/>
        public event get_venue_countCompletedEventHandler get_venue_countCompleted;
        
        /// <remarks/>
        public event get_venuesCompletedEventHandler get_venuesCompleted;
        
        /// <remarks/>
        public event get_ticketsCompletedEventHandler get_ticketsCompleted;
        
        /// <remarks/>
        public event get_ticket_infoCompletedEventHandler get_ticket_infoCompleted;
        
        /// <remarks/>
        public event get_performer_countCompletedEventHandler get_performer_countCompleted;
        
        /// <remarks/>
        public event get_performersCompletedEventHandler get_performersCompleted;
        
        /// <remarks/>
        public event purchase_orderCompletedEventHandler purchase_orderCompleted;
        
        /// <remarks/>
        public event get_events_by_zipcodeCompletedEventHandler get_events_by_zipcodeCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_drift", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("client_date_utc")]
        public string get_drift(string key, System.DateTime ts, out string server_date_utc, out int drift) {
            object[] results = this.Invoke("get_drift", new object[] {
                        key,
                        ts});
            server_date_utc = ((string)(results[1]));
            drift = ((int)(results[2]));
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void get_driftAsync(string key, System.DateTime ts) {
            this.get_driftAsync(key, ts, null);
        }
        
        /// <remarks/>
        public void get_driftAsync(string key, System.DateTime ts, object userState) {
            if ((this.get_driftOperationCompleted == null)) {
                this.get_driftOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_driftOperationCompleted);
            }
            this.InvokeAsync("get_drift", new object[] {
                        key,
                        ts}, this.get_driftOperationCompleted, userState);
        }
        
        private void Onget_driftOperationCompleted(object arg) {
            if ((this.get_driftCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_driftCompleted(this, new get_driftCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_event_count", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("get_event_countResponse")]
        public int get_event_count(string key, System.DateTime ts) {
            object[] results = this.Invoke("get_event_count", new object[] {
                        key,
                        ts});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void get_event_countAsync(string key, System.DateTime ts) {
            this.get_event_countAsync(key, ts, null);
        }
        
        /// <remarks/>
        public void get_event_countAsync(string key, System.DateTime ts, object userState) {
            if ((this.get_event_countOperationCompleted == null)) {
                this.get_event_countOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_event_countOperationCompleted);
            }
            this.InvokeAsync("get_event_count", new object[] {
                        key,
                        ts}, this.get_event_countOperationCompleted, userState);
        }
        
        private void Onget_event_countOperationCompleted(object arg) {
            if ((this.get_event_countCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_event_countCompleted(this, new get_event_countCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_events", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public @event[] get_events(string key, System.DateTime ts, int offset) {
            object[] results = this.Invoke("get_events", new object[] {
                        key,
                        ts,
                        offset});
            return ((@event[])(results[0]));
        }
        
        /// <remarks/>
        public void get_eventsAsync(string key, System.DateTime ts, int offset) {
            this.get_eventsAsync(key, ts, offset, null);
        }
        
        /// <remarks/>
        public void get_eventsAsync(string key, System.DateTime ts, int offset, object userState) {
            if ((this.get_eventsOperationCompleted == null)) {
                this.get_eventsOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_eventsOperationCompleted);
            }
            this.InvokeAsync("get_events", new object[] {
                        key,
                        ts,
                        offset}, this.get_eventsOperationCompleted, userState);
        }
        
        private void Onget_eventsOperationCompleted(object arg) {
            if ((this.get_eventsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_eventsCompleted(this, new get_eventsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_event_category_count", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("get_event_category_countResponse")]
        public int get_event_category_count(string key, System.DateTime ts) {
            object[] results = this.Invoke("get_event_category_count", new object[] {
                        key,
                        ts});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void get_event_category_countAsync(string key, System.DateTime ts) {
            this.get_event_category_countAsync(key, ts, null);
        }
        
        /// <remarks/>
        public void get_event_category_countAsync(string key, System.DateTime ts, object userState) {
            if ((this.get_event_category_countOperationCompleted == null)) {
                this.get_event_category_countOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_event_category_countOperationCompleted);
            }
            this.InvokeAsync("get_event_category_count", new object[] {
                        key,
                        ts}, this.get_event_category_countOperationCompleted, userState);
        }
        
        private void Onget_event_category_countOperationCompleted(object arg) {
            if ((this.get_event_category_countCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_event_category_countCompleted(this, new get_event_category_countCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_event_categories", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public eventcategory[] get_event_categories(string key, System.DateTime ts, int offset) {
            object[] results = this.Invoke("get_event_categories", new object[] {
                        key,
                        ts,
                        offset});
            return ((eventcategory[])(results[0]));
        }
        
        /// <remarks/>
        public void get_event_categoriesAsync(string key, System.DateTime ts, int offset) {
            this.get_event_categoriesAsync(key, ts, offset, null);
        }
        
        /// <remarks/>
        public void get_event_categoriesAsync(string key, System.DateTime ts, int offset, object userState) {
            if ((this.get_event_categoriesOperationCompleted == null)) {
                this.get_event_categoriesOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_event_categoriesOperationCompleted);
            }
            this.InvokeAsync("get_event_categories", new object[] {
                        key,
                        ts,
                        offset}, this.get_event_categoriesOperationCompleted, userState);
        }
        
        private void Onget_event_categoriesOperationCompleted(object arg) {
            if ((this.get_event_categoriesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_event_categoriesCompleted(this, new get_event_categoriesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_ticket_category_count", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("get_ticket_category_countResponse")]
        public int get_ticket_category_count(string key, System.DateTime ts) {
            object[] results = this.Invoke("get_ticket_category_count", new object[] {
                        key,
                        ts});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void get_ticket_category_countAsync(string key, System.DateTime ts) {
            this.get_ticket_category_countAsync(key, ts, null);
        }
        
        /// <remarks/>
        public void get_ticket_category_countAsync(string key, System.DateTime ts, object userState) {
            if ((this.get_ticket_category_countOperationCompleted == null)) {
                this.get_ticket_category_countOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_ticket_category_countOperationCompleted);
            }
            this.InvokeAsync("get_ticket_category_count", new object[] {
                        key,
                        ts}, this.get_ticket_category_countOperationCompleted, userState);
        }
        
        private void Onget_ticket_category_countOperationCompleted(object arg) {
            if ((this.get_ticket_category_countCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_ticket_category_countCompleted(this, new get_ticket_category_countCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_ticket_categories", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public ticketcategory[] get_ticket_categories(string key, System.DateTime ts, int offset) {
            object[] results = this.Invoke("get_ticket_categories", new object[] {
                        key,
                        ts,
                        offset});
            return ((ticketcategory[])(results[0]));
        }
        
        /// <remarks/>
        public void get_ticket_categoriesAsync(string key, System.DateTime ts, int offset) {
            this.get_ticket_categoriesAsync(key, ts, offset, null);
        }
        
        /// <remarks/>
        public void get_ticket_categoriesAsync(string key, System.DateTime ts, int offset, object userState) {
            if ((this.get_ticket_categoriesOperationCompleted == null)) {
                this.get_ticket_categoriesOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_ticket_categoriesOperationCompleted);
            }
            this.InvokeAsync("get_ticket_categories", new object[] {
                        key,
                        ts,
                        offset}, this.get_ticket_categoriesOperationCompleted, userState);
        }
        
        private void Onget_ticket_categoriesOperationCompleted(object arg) {
            if ((this.get_ticket_categoriesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_ticket_categoriesCompleted(this, new get_ticket_categoriesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_venue_count", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("get_venue_countResponse")]
        public int get_venue_count(string key, System.DateTime ts) {
            object[] results = this.Invoke("get_venue_count", new object[] {
                        key,
                        ts});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void get_venue_countAsync(string key, System.DateTime ts) {
            this.get_venue_countAsync(key, ts, null);
        }
        
        /// <remarks/>
        public void get_venue_countAsync(string key, System.DateTime ts, object userState) {
            if ((this.get_venue_countOperationCompleted == null)) {
                this.get_venue_countOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_venue_countOperationCompleted);
            }
            this.InvokeAsync("get_venue_count", new object[] {
                        key,
                        ts}, this.get_venue_countOperationCompleted, userState);
        }
        
        private void Onget_venue_countOperationCompleted(object arg) {
            if ((this.get_venue_countCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_venue_countCompleted(this, new get_venue_countCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_venues", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public venue[] get_venues(string key, System.DateTime ts, int offset) {
            object[] results = this.Invoke("get_venues", new object[] {
                        key,
                        ts,
                        offset});
            return ((venue[])(results[0]));
        }
        
        /// <remarks/>
        public void get_venuesAsync(string key, System.DateTime ts, int offset) {
            this.get_venuesAsync(key, ts, offset, null);
        }
        
        /// <remarks/>
        public void get_venuesAsync(string key, System.DateTime ts, int offset, object userState) {
            if ((this.get_venuesOperationCompleted == null)) {
                this.get_venuesOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_venuesOperationCompleted);
            }
            this.InvokeAsync("get_venues", new object[] {
                        key,
                        ts,
                        offset}, this.get_venuesOperationCompleted, userState);
        }
        
        private void Onget_venuesOperationCompleted(object arg) {
            if ((this.get_venuesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_venuesCompleted(this, new get_venuesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_tickets", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public ticket[] get_tickets(string key, int event_id) {
            object[] results = this.Invoke("get_tickets", new object[] {
                        key,
                        event_id});
            return ((ticket[])(results[0]));
        }
        
        /// <remarks/>
        public void get_ticketsAsync(string key, int event_id) {
            this.get_ticketsAsync(key, event_id, null);
        }
        
        /// <remarks/>
        public void get_ticketsAsync(string key, int event_id, object userState) {
            if ((this.get_ticketsOperationCompleted == null)) {
                this.get_ticketsOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_ticketsOperationCompleted);
            }
            this.InvokeAsync("get_tickets", new object[] {
                        key,
                        event_id}, this.get_ticketsOperationCompleted, userState);
        }
        
        private void Onget_ticketsOperationCompleted(object arg) {
            if ((this.get_ticketsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_ticketsCompleted(this, new get_ticketsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_ticket_info", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public ticket_info get_ticket_info(string key, int ticket_id) {
            object[] results = this.Invoke("get_ticket_info", new object[] {
                        key,
                        ticket_id});
            return ((ticket_info)(results[0]));
        }
        
        /// <remarks/>
        public void get_ticket_infoAsync(string key, int ticket_id) {
            this.get_ticket_infoAsync(key, ticket_id, null);
        }
        
        /// <remarks/>
        public void get_ticket_infoAsync(string key, int ticket_id, object userState) {
            if ((this.get_ticket_infoOperationCompleted == null)) {
                this.get_ticket_infoOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_ticket_infoOperationCompleted);
            }
            this.InvokeAsync("get_ticket_info", new object[] {
                        key,
                        ticket_id}, this.get_ticket_infoOperationCompleted, userState);
        }
        
        private void Onget_ticket_infoOperationCompleted(object arg) {
            if ((this.get_ticket_infoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_ticket_infoCompleted(this, new get_ticket_infoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_performer_count", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("get_performer_countResponse")]
        public int get_performer_count(string key, System.DateTime ts) {
            object[] results = this.Invoke("get_performer_count", new object[] {
                        key,
                        ts});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void get_performer_countAsync(string key, System.DateTime ts) {
            this.get_performer_countAsync(key, ts, null);
        }
        
        /// <remarks/>
        public void get_performer_countAsync(string key, System.DateTime ts, object userState) {
            if ((this.get_performer_countOperationCompleted == null)) {
                this.get_performer_countOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_performer_countOperationCompleted);
            }
            this.InvokeAsync("get_performer_count", new object[] {
                        key,
                        ts}, this.get_performer_countOperationCompleted, userState);
        }
        
        private void Onget_performer_countOperationCompleted(object arg) {
            if ((this.get_performer_countCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_performer_countCompleted(this, new get_performer_countCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_performers", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public performer[] get_performers(string key, System.DateTime ts, int offset) {
            object[] results = this.Invoke("get_performers", new object[] {
                        key,
                        ts,
                        offset});
            return ((performer[])(results[0]));
        }
        
        /// <remarks/>
        public void get_performersAsync(string key, System.DateTime ts, int offset) {
            this.get_performersAsync(key, ts, offset, null);
        }
        
        /// <remarks/>
        public void get_performersAsync(string key, System.DateTime ts, int offset, object userState) {
            if ((this.get_performersOperationCompleted == null)) {
                this.get_performersOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_performersOperationCompleted);
            }
            this.InvokeAsync("get_performers", new object[] {
                        key,
                        ts,
                        offset}, this.get_performersOperationCompleted, userState);
        }
        
        private void Onget_performersOperationCompleted(object arg) {
            if ((this.get_performersCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_performersCompleted(this, new get_performersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/purchase_order", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("purchase_orderResponse")]
        public int purchase_order(string key, string event_name, int ticket_category_id, int ticket_id, int ticket_quantity, string order_number, string customer_address, string customer_phone) {
            object[] results = this.Invoke("purchase_order", new object[] {
                        key,
                        event_name,
                        ticket_category_id,
                        ticket_id,
                        ticket_quantity,
                        order_number,
                        customer_address,
                        customer_phone});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void purchase_orderAsync(string key, string event_name, int ticket_category_id, int ticket_id, int ticket_quantity, string order_number, string customer_address, string customer_phone) {
            this.purchase_orderAsync(key, event_name, ticket_category_id, ticket_id, ticket_quantity, order_number, customer_address, customer_phone, null);
        }
        
        /// <remarks/>
        public void purchase_orderAsync(string key, string event_name, int ticket_category_id, int ticket_id, int ticket_quantity, string order_number, string customer_address, string customer_phone, object userState) {
            if ((this.purchase_orderOperationCompleted == null)) {
                this.purchase_orderOperationCompleted = new System.Threading.SendOrPostCallback(this.Onpurchase_orderOperationCompleted);
            }
            this.InvokeAsync("purchase_order", new object[] {
                        key,
                        event_name,
                        ticket_category_id,
                        ticket_id,
                        ticket_quantity,
                        order_number,
                        customer_address,
                        customer_phone}, this.purchase_orderOperationCompleted, userState);
        }
        
        private void Onpurchase_orderOperationCompleted(object arg) {
            if ((this.purchase_orderCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.purchase_orderCompleted(this, new purchase_orderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://plugin.xtics.com/rpc-dev/soap.php/get_events_by_zipcode", RequestNamespace="urn:xtics_rpc", ResponseNamespace="urn:xtics_rpc")]
        [return: System.Xml.Serialization.SoapElementAttribute("get_events_by_zipcodeResponse")]
        public @event[] get_events_by_zipcode(string key, string zipcode, System.DateTime start_date, int number_of_days) {
            object[] results = this.Invoke("get_events_by_zipcode", new object[] {
                        key,
                        zipcode,
                        start_date,
                        number_of_days});
            return ((@event[])(results[0]));
        }
        
        /// <remarks/>
        public void get_events_by_zipcodeAsync(string key, string zipcode, System.DateTime start_date, int number_of_days) {
            this.get_events_by_zipcodeAsync(key, zipcode, start_date, number_of_days, null);
        }
        
        /// <remarks/>
        public void get_events_by_zipcodeAsync(string key, string zipcode, System.DateTime start_date, int number_of_days, object userState) {
            if ((this.get_events_by_zipcodeOperationCompleted == null)) {
                this.get_events_by_zipcodeOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_events_by_zipcodeOperationCompleted);
            }
            this.InvokeAsync("get_events_by_zipcode", new object[] {
                        key,
                        zipcode,
                        start_date,
                        number_of_days}, this.get_events_by_zipcodeOperationCompleted, userState);
        }
        
        private void Onget_events_by_zipcodeOperationCompleted(object arg) {
            if ((this.get_events_by_zipcodeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_events_by_zipcodeCompleted(this, new get_events_by_zipcodeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30128.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:xtics_rpc")]
    public partial class @event {
        
        private int idField;
        
        private string nameField;
        
        private int event_category_idField;
        
        private int venue_idField;
        
        private System.DateTime dateField;
        
        private string timeField;
        
        private int has_ticketsField;
        
        private int performer1Field;
        
        private int performer2Field;
        
        /// <remarks/>
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public int event_category_id {
            get {
                return this.event_category_idField;
            }
            set {
                this.event_category_idField = value;
            }
        }
        
        /// <remarks/>
        public int venue_id {
            get {
                return this.venue_idField;
            }
            set {
                this.venue_idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(DataType="date")]
        public System.DateTime date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
            }
        }
        
        /// <remarks/>
        public string time {
            get {
                return this.timeField;
            }
            set {
                this.timeField = value;
            }
        }
        
        /// <remarks/>
        public int has_tickets {
            get {
                return this.has_ticketsField;
            }
            set {
                this.has_ticketsField = value;
            }
        }
        
        /// <remarks/>
        public int performer1 {
            get {
                return this.performer1Field;
            }
            set {
                this.performer1Field = value;
            }
        }
        
        /// <remarks/>
        public int performer2 {
            get {
                return this.performer2Field;
            }
            set {
                this.performer2Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30128.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:xtics_rpc")]
    public partial class performer {
        
        private int idField;
        
        private string nameField;
        
        private int event_category_idField;
        
        /// <remarks/>
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public int event_category_id {
            get {
                return this.event_category_idField;
            }
            set {
                this.event_category_idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30128.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:xtics_rpc")]
    public partial class ticket_info {
        
        private int idField;
        
        private int event_idField;
        
        private int venue_idField;
        
        private string eventField;
        
        private string opponentField;
        
        private string venueField;
        
        private int qtyField;
        
        private System.DateTime dateField;
        
        private string sectionField;
        
        private string rowField;
        
        private string seat_fromField;
        
        private string seat_thruField;
        
        private string descriptionField;
        
        private double priceField;
        
        private string typeField;
        
        /// <remarks/>
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public int event_id {
            get {
                return this.event_idField;
            }
            set {
                this.event_idField = value;
            }
        }
        
        /// <remarks/>
        public int venue_id {
            get {
                return this.venue_idField;
            }
            set {
                this.venue_idField = value;
            }
        }
        
        /// <remarks/>
        public string @event {
            get {
                return this.eventField;
            }
            set {
                this.eventField = value;
            }
        }
        
        /// <remarks/>
        public string opponent {
            get {
                return this.opponentField;
            }
            set {
                this.opponentField = value;
            }
        }
        
        /// <remarks/>
        public string venue {
            get {
                return this.venueField;
            }
            set {
                this.venueField = value;
            }
        }
        
        /// <remarks/>
        public int qty {
            get {
                return this.qtyField;
            }
            set {
                this.qtyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(DataType="date")]
        public System.DateTime date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
            }
        }
        
        /// <remarks/>
        public string section {
            get {
                return this.sectionField;
            }
            set {
                this.sectionField = value;
            }
        }
        
        /// <remarks/>
        public string row {
            get {
                return this.rowField;
            }
            set {
                this.rowField = value;
            }
        }
        
        /// <remarks/>
        public string seat_from {
            get {
                return this.seat_fromField;
            }
            set {
                this.seat_fromField = value;
            }
        }
        
        /// <remarks/>
        public string seat_thru {
            get {
                return this.seat_thruField;
            }
            set {
                this.seat_thruField = value;
            }
        }
        
        /// <remarks/>
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public double price {
            get {
                return this.priceField;
            }
            set {
                this.priceField = value;
            }
        }
        
        /// <remarks/>
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30128.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:xtics_rpc")]
    public partial class ticket {
        
        private int idField;
        
        private int qtyField;
        
        private System.DateTime dateField;
        
        private string sectionField;
        
        private string rowField;
        
        private string seat_fromField;
        
        private string seat_thruField;
        
        private string descriptionField;
        
        private double priceField;
        
        /// <remarks/>
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public int qty {
            get {
                return this.qtyField;
            }
            set {
                this.qtyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(DataType="date")]
        public System.DateTime date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
            }
        }
        
        /// <remarks/>
        public string section {
            get {
                return this.sectionField;
            }
            set {
                this.sectionField = value;
            }
        }
        
        /// <remarks/>
        public string row {
            get {
                return this.rowField;
            }
            set {
                this.rowField = value;
            }
        }
        
        /// <remarks/>
        public string seat_from {
            get {
                return this.seat_fromField;
            }
            set {
                this.seat_fromField = value;
            }
        }
        
        /// <remarks/>
        public string seat_thru {
            get {
                return this.seat_thruField;
            }
            set {
                this.seat_thruField = value;
            }
        }
        
        /// <remarks/>
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public double price {
            get {
                return this.priceField;
            }
            set {
                this.priceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30128.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:xtics_rpc")]
    public partial class venue {
        
        private int idField;
        
        private string nameField;
        
        private string cityField;
        
        private string provinceField;
        
        private string countryField;
        
        private string address1Field;
        
        private string address2Field;
        
        private string phoneField;
        
        /// <remarks/>
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string city {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
            }
        }
        
        /// <remarks/>
        public string province {
            get {
                return this.provinceField;
            }
            set {
                this.provinceField = value;
            }
        }
        
        /// <remarks/>
        public string country {
            get {
                return this.countryField;
            }
            set {
                this.countryField = value;
            }
        }
        
        /// <remarks/>
        public string address1 {
            get {
                return this.address1Field;
            }
            set {
                this.address1Field = value;
            }
        }
        
        /// <remarks/>
        public string address2 {
            get {
                return this.address2Field;
            }
            set {
                this.address2Field = value;
            }
        }
        
        /// <remarks/>
        public string phone {
            get {
                return this.phoneField;
            }
            set {
                this.phoneField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30128.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:xtics_rpc")]
    public partial class ticketcategory {
        
        private int idField;
        
        private int event_idField;
        
        private string descriptionField;
        
        private string detailField;
        
        private double priceField;
        
        /// <remarks/>
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public int event_id {
            get {
                return this.event_idField;
            }
            set {
                this.event_idField = value;
            }
        }
        
        /// <remarks/>
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public string detail {
            get {
                return this.detailField;
            }
            set {
                this.detailField = value;
            }
        }
        
        /// <remarks/>
        public double price {
            get {
                return this.priceField;
            }
            set {
                this.priceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30128.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:xtics_rpc")]
    public partial class eventcategory {
        
        private int idField;
        
        private string nameField;
        
        private bool activeField;
        
        /// <remarks/>
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public bool active {
            get {
                return this.activeField;
            }
            set {
                this.activeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_driftCompletedEventHandler(object sender, get_driftCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_driftCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_driftCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public string server_date_utc {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
        
        /// <remarks/>
        public int drift {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[2]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_event_countCompletedEventHandler(object sender, get_event_countCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_event_countCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_event_countCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_eventsCompletedEventHandler(object sender, get_eventsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_eventsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_eventsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public @event[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((@event[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_event_category_countCompletedEventHandler(object sender, get_event_category_countCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_event_category_countCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_event_category_countCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_event_categoriesCompletedEventHandler(object sender, get_event_categoriesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_event_categoriesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_event_categoriesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public eventcategory[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((eventcategory[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_ticket_category_countCompletedEventHandler(object sender, get_ticket_category_countCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_ticket_category_countCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_ticket_category_countCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_ticket_categoriesCompletedEventHandler(object sender, get_ticket_categoriesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_ticket_categoriesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_ticket_categoriesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ticketcategory[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ticketcategory[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_venue_countCompletedEventHandler(object sender, get_venue_countCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_venue_countCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_venue_countCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_venuesCompletedEventHandler(object sender, get_venuesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_venuesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_venuesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public venue[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((venue[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_ticketsCompletedEventHandler(object sender, get_ticketsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_ticketsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_ticketsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ticket[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ticket[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_ticket_infoCompletedEventHandler(object sender, get_ticket_infoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_ticket_infoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_ticket_infoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ticket_info Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ticket_info)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_performer_countCompletedEventHandler(object sender, get_performer_countCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_performer_countCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_performer_countCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_performersCompletedEventHandler(object sender, get_performersCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_performersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_performersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public performer[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((performer[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void purchase_orderCompletedEventHandler(object sender, purchase_orderCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class purchase_orderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal purchase_orderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    public delegate void get_events_by_zipcodeCompletedEventHandler(object sender, get_events_by_zipcodeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30128.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_events_by_zipcodeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_events_by_zipcodeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public @event[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((@event[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591