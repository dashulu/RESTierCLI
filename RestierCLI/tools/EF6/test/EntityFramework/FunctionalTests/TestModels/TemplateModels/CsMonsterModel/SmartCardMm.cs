//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FunctionalTests.ProductivityApi.TemplateModels.CsMonsterModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class SmartCardMm
    {
        public string Username { get; set; }
        public string CardSerial { get; set; }
        public System.DateTime Issued { get; set; }
    
        public virtual Another.Place.LoginMm Login { get; set; }
        public virtual LastLoginMm LastLogin { get; set; }
    }
}
