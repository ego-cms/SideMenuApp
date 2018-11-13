using System;
namespace SideMenuApp.SideMenu
{
    /// <summary>
    /// Exception that's thrown when Side menu doesn't have appropriate type
    /// </summary>
    public class SideMenuInitializationException : Exception
    {
        public SideMenuInitializationException()
        {
        }

        public SideMenuInitializationException(string message) : base(message)
        {
        }

        public SideMenuInitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SideMenuInitializationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }

}
