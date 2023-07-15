// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MVC.Areas.Identity.Models.Manage
{
    public class IndexViewModel
    {
        public EditExtraProfileModel profile { get; set; } = default!;
        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        public string AuthenticatorKey { get; set; } = default!;
    }
}
