using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace IdentityDemo.Kakao;

public static class KakaoTalkAuthenticationExtensions
{
    public static AuthenticationBuilder AddKakaoTalk1([NotNull] this AuthenticationBuilder builder)
    {
        return builder.AddKakaoTalk1(KakaoTalkAuthenticationDefaults.AuthenticationScheme, options => { });
    }

    public static AuthenticationBuilder AddKakaoTalk1(
       [NotNull] this AuthenticationBuilder builder,
       [NotNull] string scheme,
       [NotNull] Action<KakaoTalkAuthenticationOptions> configuration)
    {
        return builder.AddKakaoTalk1(scheme, KakaoTalkAuthenticationDefaults.DisplayName, configuration);
    }

    public static AuthenticationBuilder AddKakaoTalk1(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] string scheme,
        [CanBeNull] string caption,
        [NotNull] Action<KakaoTalkAuthenticationOptions> configuration)
    {

        return builder.AddOAuth<KakaoTalkAuthenticationOptions, KakaoTalkAuthenticationHandler>(scheme, caption, configuration);
    }
}
