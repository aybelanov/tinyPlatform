using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hub.Web.Areas.OpenId.Models;

/// <summary>
/// <see href= "https://openid.net/specs/openid-connect-discovery-1_0.html#ProviderMetadata" />
/// </summary>
public partial record OpenIdConfigModel
{
   #region REQUIRED

   /// <summary>
   /// REQUIRED. URL using the https scheme with no query or fragment component that the OP asserts as its Issuer Identifier.
   /// If Issuer discovery is supported (see Section 2), this value MUST be identical to the issuer value returned by WebFinger.
   /// This also MUST be identical to the iss Claim value in ID Tokens issued from this Issuer.
   /// </summary>
   [JsonProperty(PropertyName = "issuer")]
   public string Issuer { get; set; }


   /// <summary>
   /// REQUIRED. URL of the OP's JSON Web Key Set <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWK">[JWK]</see> document. 
   /// This contains the signing key(s) the RP uses to validate signatures from the OP.
   /// The JWK Set MAY also contain the Server's encryption key(s), which are used by RPs to encrypt requests to the Server.
   /// When both signing and encryption keys are made available,
   /// a use (Key Use) parameter value is REQUIRED for all keys in the referenced JWK Set to indicate each key's intended usage.
   /// Although some algorithms allow the same key to be used for both signatures and encryption, doing so is NOT RECOMMENDED, as it is less secure.
   /// The JWK x5c parameter MAY be used to provide X.509 representations of keys provided.
   /// When used, the bare key values MUST still be present and MUST match those in the certificate.
   /// </summary>
   [JsonProperty(PropertyName = "jwks_uri")]
   public string JwksUri { get; set; }


   /// <summary>
   /// REQUIRED. URL of the OP's OAuth 2.0 Authorization Endpoint 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Core">[OpenID.Core]</see>.
   /// </summary>
   [JsonProperty(PropertyName = "authorization_endpoint")]
   public string AuthorizationEndPoint { get; set; }


   /// <summary>
   /// URL of the OP's OAuth 2.0 Token Endpoint [OpenID.Core]. 
   /// This is REQUIRED unless only the Implicit Flow is used.
   /// </summary>
   [JsonProperty(PropertyName = "token_endpoint")]
   public string TokenEndPoint { get; set; }


   /// <summary>
   /// REQUIRED. URL of an OP iframe that supports cross-origin communications for session state information with the RP Client, using the HTML5 postMessage API. 
   /// This URL MUST use the https scheme and MAY contain port, path, and query parameter components. 
   /// The page is loaded from an invisible iframe embedded in an RP page so that it can run in the OP's security context. 
   /// It accepts postMessage requests from the relevant RP iframe and uses postMessage to post back the login status of the End-User at the OP.
   /// </summary>
   /// <remarks><see href="https://openid.net/specs/openid-connect-session-1_0.html"/></remarks>
   [JsonProperty(PropertyName = "check_session_iframe")]
   public string CheckSessionIframe { get; set; }


   /// <summary>
   /// REQUIRED. JSON array containing a list of the JWS signing algorithms (alg values) supported by the OP for the ID Token to encode the Claims in a JWT 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWT">[JWT]</see>.
   /// The algorithm RS256 MUST be included. The value none MAY be supported,
   /// but MUST NOT be used unless the Response Type used returns no ID Token from the Authorization Endpoint (such as when using the Authorization Code Flow).
   /// </summary>
   [JsonProperty(PropertyName = "id_token_signing_alg_values_supported")]
   public IEnumerable<string> IdTokenSigningAlgValuesSupported { get; set; }


   /// <summary>
   /// REQUIRED. JSON array containing a list of the OAuth 2.0 response_type values that this OP supports.
   /// Dynamic OpenID Providers MUST support the code, id_token, and the token id_token Response Type values.
   /// </summary>
   [JsonProperty(PropertyName = "response_types_supported")]
   public IEnumerable<string> ResponseTypesSupported { get; set; }


   /// <summary>
   /// REQUIRED. JSON array containing a list of the Subject Identifier types that this OP supports. Valid types include pairwise and public.
   /// </summary>
   [JsonProperty(PropertyName = "subject_types_supported")]
   public IEnumerable<string> SubjectTypesSupported { get; set; }


   #endregion


   #region RECOMMENDED

   /// <summary>
   /// RECOMMENDED. URL of the OP's UserInfo Endpoint <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Core">[OpenID.Core]</see>. 
   /// This URL MUST use the https scheme and MAY contain port, path, and query parameter components.
   /// </summary>
   [JsonProperty(PropertyName = "userinfo_endpoint")]
   public string UserInfoEndPoint { get; set; }


   /// <summary>
   /// RECOMMENDED. URL of the OP's Dynamic Client Registration Endpoint
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Registration">[OpenID.Registration]</see>.
   /// </summary>
   [JsonProperty(PropertyName = "registration_endpoint")]
   public string RegistrationEndpoint { get; set; }


   /// <summary>
   /// RECOMMENDED. JSON array containing a list of the OAuth 2.0 <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#RFC6749">[RFC6749]</see>
   /// scope values that this server supports.
   /// The server MUST support the openid scope value. Servers MAY choose not to advertise some supported scope values even when this parameter is used,
   /// although those defined in <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Core">[OpenID.Core]</see> SHOULD be listed, if supported.
   /// </summary>
   [JsonProperty(PropertyName = "scopes_supported")]
   public IEnumerable<string> ScopesSupported { get; set; }


   /// <summary>
   /// RECOMMENDED. JSON array containing a list of the Claim Names of the Claims that the OpenID Provider MAY be able to supply values for.
   /// Note that for privacy or other reasons, this might not be an exhaustive list.
   /// </summary>
   [JsonProperty(PropertyName = "claims_supported")]
   public IEnumerable<string> ClaimsSupported { get; set; }

   #endregion


   #region OPTIONAL

   /// <summary>
   /// OPTIONAL. JSON array containing a list of the Claim Types that the OpenID Provider supports.
   /// These Claim Types are described in Section 5.6 of 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Core">OpenID Connect Core 1.0 [OpenID.Core]</see>.
   /// Values defined by this specification are normal, aggregated, and distributed.
   /// If omitted, the implementation supports only normal Claims.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "claim_types_supported")]
   public IEnumerable<string> ClaimTypesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. Boolean value specifying whether the OP supports use of the claims parameter, with true indicating support. If omitted, the default value is false.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "claims_parameter_supported")]
   public bool ClaimsParameterSupported { get; set; }


   /// <summary>
   /// OPTIONAL. Languages and scripts supported for values in Claims being returned, represented as a JSON array of 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#RFC5646">BCP47</see>
   /// [RFC5646] language tag values.
   /// Not all languages and scripts are necessarily supported for all Claim values.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "claims_locales_supported")]
   public IEnumerable<string> ClaimsLocalesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the OAuth 2.0 Grant Type values that this OP supports.
   /// Dynamic OpenID Providers MUST support the authorization_code and implicit Grant Type values and MAY support other Grant Types.
   /// If omitted, the default value is ["authorization_code", "implicit"].
   /// </summary>
   [JsonProperty(PropertyName = "grant_types_supported")]
   public IEnumerable<string> GrantTypesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the OAuth 2.0 response_mode values that this OP supports,
   /// as specified in OAuth 2.0 Multiple 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OAuth.Responses">Response Type Encoding Practices [OAuth.Responses]</see>.
   /// If omitted, the default for Dynamic OpenID Providers is ["query", "fragment"].
   /// </summary>
   [JsonProperty(PropertyName = "response_modes_supported")]
   public IEnumerable<string> ResponseModesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the Authentication Context Class References that this OP supports.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "acr_values_supported")]
   public IEnumerable<string> AcrValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of Client Authentication methods supported by this Token Endpoint.
   /// The options are client_secret_post, client_secret_basic, client_secret_jwt, and private_key_jwt, as described in Section 9 of
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Core">OpenID Connect Core 1.0 [OpenID.Core]</see>. 
   /// Other authentication methods MAY be defined by extensions. If omitted, the default is client_secret_basic -- the HTTP Basic Authentication Scheme
   /// specified in Section 2.3.1 of 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#RFC6749">OAuth 2.0 [RFC6749]</see>.
   /// </summary>
   [JsonProperty(PropertyName = "token_endpoint_auth_methods_supported")]
   public IEnumerable<string> TokenEndpointAuthMethodsSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWE encryption algorithms (alg values) supported by the OP for the ID Token to encode the Claims in a JWT 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWT">[JWT]</see>.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "id_token_encryption_alg_values_supported")]
   public IEnumerable<string> IdTokenEncryptionAlgValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWE encryption algorithms (enc values) supported by the OP for the ID Token to encode the Claims in a JWT
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWT">[JWT]</see>.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "id_token_encryption_enc_values_supported")]
   public IEnumerable<string> IdTokenEncryptionEncValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWS
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWS">[JWS]</see>
   /// signing algorithms (alg values)
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWA">[JWA]</see>
   /// supported by the UserInfo Endpoint to encode the Claims in a JWT 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWT">[JWT]</see>.
   /// The value none MAY be included.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "userinfo_signing_alg_values_supported")]
   public IEnumerable<string> UserInfoSigningAlgValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWE 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWE">[JWE]</see>
   /// encryption algorithms (alg values) 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWA">[JWA]</see>  
   /// supported by the UserInfo Endpoint to encode the Claims in a JWT 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWT">[JWT]</see>.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "userinfo_encryption_alg_values_supported")]
   public IEnumerable<string> UserInfoEncryptionAlgValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWE encryption algorithms (enc values) 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWA">[JWA]</see> 
   /// supported by the UserInfo Endpoint to encode the Claims in a JWT 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWT">[JWT]</see>.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "userinfo_encryption_enc_values_supported")]
   public IEnumerable<string> UserInfoEncryptionEncValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. Boolean value specifying whether the OP supports use of the request parameter, with true indicating support. If omitted, the default value is false.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "request_parameter_supported")]
   public bool RequestParameterSupported { get; set; }


   /// <summary>
   /// OPTIONAL. Boolean value specifying whether the OP supports use of the request_uri parameter, with true indicating support. If omitted, the default value is true.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "request_uri_parameter_supported")]
   public bool RequestUriParameterSupported { get; set; }


   /// <summary>
   /// OPTIONAL. Boolean value specifying whether the OP requires any request_uri values used to be pre-registered using the request_uris registration parameter.
   /// Pre-registration is REQUIRED when the value is true. If omitted, the default value is false.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "require_request_uri_registration")]
   public bool RequireRequestUriRegistration { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWS signing algorithms (alg values) supported by the OP for Request Objects, 
   /// which are described in Section 6.1 of 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Core">OpenID Connect Core 1.0 [OpenID.Core]</see>.
   /// These algorithms are used both when the Request Object is passed by value (using the request parameter)
   /// and when it is passed by reference (using the request_uri parameter). Servers SHOULD support none and RS256.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "request_object_signing_alg_values_supported")]
   public IEnumerable<string> RequestObjectSigningAlgValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWE encryption algorithms (alg values) supported by the OP for Request Objects.
   /// These algorithms are used both when the Request Object is passed by value and when it is passed by reference.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "request_object_encryption_alg_values_supported")]
   public IEnumerable<string> RequestObjectEncryptionAlgValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWE encryption algorithms (enc values) supported by the OP for Request Objects.
   /// These algorithms are used both when the Request Object is passed by value and when it is passed by reference.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "request_object_encryption_enc_values_supported")]
   public IEnumerable<string> RequestObjectEncryptionEncValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the JWS signing algorithms (alg values) supported by the Token Endpoint for the signature on the JWT 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#JWT">[JWT]</see>
   /// used to authenticate the Client at the Token Endpoint for the private_key_jwt and client_secret_jwt authentication methods.
   /// Servers SHOULD support RS256. The value none MUST NOT be used.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "token_endpoint_auth_signing_alg_values_supported")]
   public IEnumerable<string> TokenEndpointAuthSigningAlgValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. JSON array containing a list of the display parameter values that the OpenID Provider supports.
   /// These values are described in Section 3.1.2.1 of 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#OpenID.Core">OpenID Connect Core 1.0 [OpenID.Core]</see>.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "display_values_supported")]
   public IEnumerable<string> DisplayValuesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. Languages and scripts supported for the user interface, represented as a JSON array of 
   /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html#RFC5646">BCP47</see>
   /// [RFC5646] language tag values.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "ui_locales_supported")]
   public IEnumerable<string> UiLocalesSupported { get; set; }


   /// <summary>
   /// OPTIONAL. URL that the OpenID Provider provides to the person registering the Client to read about the OP's requirements
   /// on how the Relying Party can use the data provided by the OP.
   /// The registration process SHOULD display this URL to the person registering the Client if it is given.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "op_policy_uri")]
   public string OpPolicyUri { get; set; }


   /// <summary>
   /// OPTIONAL. URL that the OpenID Provider provides to the person registering the Client to read about OpenID Provider's terms of service.
   /// The registration process SHOULD display this URL to the person registering the Client if it is given.
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "op_tos_uri")]
   public string OpTosUri { get; set; }
   #endregion


   #region NON-NORMATIVE (CUSTOM)

   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   /// <remarks><see href="https://openid.net/specs/openid-connect-session-1_0.html"/></remarks>
   [JsonProperty(PropertyName = "end_session_endpoint")]
   public string EndSessionEndPoint { get; set; }

   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "code_challenge_methods_supported")]
   public IEnumerable<string> CodeChallengeMethodsSupported { get; set; }

   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "revocation_endpoint")]
   public string RevocationEndPoint { get; set; }


   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "introspection_endpoint")]
   public string IntrospectionEndPoint { get; set; }


   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "device_authorization_endpoint")]
   public string DeviceAuthorizationEndPoint { get; set; }


   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "frontchannel_logout_supported")]
   public bool FrontChannelLogoutSupported { get; set; }


   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "frontchannel_logout_session_supported")]
   public bool FrontChannelLogoutSessionSupported { get; set; }


   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "backchannel_logout_supported")]
   public bool BackChannelLogoutSupported { get; set; }


   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "backchannel_logout_session_supported")]
   public bool BackChannelLogoutSessionSupported { get; set; }


   /// <summary>
   /// NON-NORMATIVE
   /// </summary>
   [JsonIgnore]
   [JsonProperty(PropertyName = "authorization_response_iss_parameter_supported")]
   public bool AuthorizationResponseIssParameterSupported { get; set; }

   #endregion
}
