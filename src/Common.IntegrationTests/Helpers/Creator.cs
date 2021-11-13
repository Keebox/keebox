using AutoFixture;


namespace Keebox.Common.IntegrationTests.Helpers
{
	public static class Creator
	{
		private static readonly Fixture _fixture = new();

		public static string CreateStringWithMaxLength(int length)
		{
			return string.Join(string.Empty, _fixture.CreateMany<char>(length));
		}
	}
}