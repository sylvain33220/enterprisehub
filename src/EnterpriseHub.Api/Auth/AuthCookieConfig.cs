/*
* @file  : AuthCookieConfig.cs
* @brief : Configuration for authentication cookies, including refresh token settings.
* @date  : 2024-06-01
*@author : Poteaux sylvain
*/
namespace EnterpriseHub.Api.Auth;

public class CookieConfig
{
    public string RefreshCookieName { get; set; } = "eh_rt";
    public int RefreshTokenExpiryDays { get; set; } = 14;
    public bool Secure { get; set; } = true;
    public string SameSite { get; set; } = "Strict";
}