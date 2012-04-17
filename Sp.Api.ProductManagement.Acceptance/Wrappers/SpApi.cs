using HtmlAgilityPack;
using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	public class SpApi
	{
		readonly RestClient _client;
		readonly CookieContainer _cookieContainer;
		readonly string _baseUrl;
		readonly string _username;
		readonly string _password;

		public SpApi( SpApiConfiguration apiConfiguration )
		{
			_baseUrl = apiConfiguration.BaseUrl;
			_cookieContainer = new System.Net.CookieContainer();
			_username = apiConfiguration.Username;
			_password = apiConfiguration.Password;
			_client = new RestClient();
			_client.BaseUrl = _baseUrl;
			_client.CookieContainer = _cookieContainer;
		}

		internal void ExecuteLogin()
		{
			var loginPage = _client.Execute( new RestRequest( "login.aspx", Method.GET ) );
			var loginPageHtml = new HtmlDocument();
			loginPageHtml.LoadHtml( loginPage.Content );
			RestRequest request = new RestRequest( "login.aspx", Method.POST );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oLoginBtn.x", "0" );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oLoginBtn.y", "0" );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oUserNameTb", _username );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oPasswordTb", _password );
			request.AddHiddenFieldValueFrom( "__EVENTVALIDATION", loginPageHtml );
			request.AddHiddenFieldValueFrom( "__VIEWSTATE", loginPageHtml );
			var loginResult = _client.Execute( request );
			if ( !loginResult.Content.Contains( "createActivationKeysTitle" ) )
			{
				Console.WriteLine( "FAILED" );
				Console.WriteLine( loginResult.Content );
				Console.WriteLine( loginResult.ResponseUri );
			}
		}

		protected IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			return _client.Execute<T>( request );
		}

		public override string ToString()
		{
			return string.Format( "{0}( Url: {1}; Username: {2})",
			  typeof( SpApi ).Name, _baseUrl, _username );
		}
	}

	static class RestRequestFormSubmissionExtensions
	{
		public static void AddHiddenFieldValueFrom( this RestRequest that, string fieldName, HtmlDocument htmlDocument )
		{
			that.AddParameter( fieldName, htmlDocument.HiddenInputValue( fieldName ) );
		}
	}

	static class HtmlDocumentExtensions
	{
		public static string HiddenInputValue( this HtmlDocument page, string fieldName )
		{
			var existingNode = page
				.DocumentNode.Descendants( "input" )
				.SingleOrDefault( i => i.HasAttributeValue( fieldName, "name" ) );
			string existingValue = existingNode == null ? String.Empty : existingNode.Attributes[ "value" ].Value;
			return existingValue;
		}

		public static bool HasAttributeValue( this HtmlNode node, string attributeValue, string attributeName )
		{
			return node.Attributes.Contains( attributeName ) && node.Attributes[ attributeName ].Value == attributeValue;
		}
	}
}