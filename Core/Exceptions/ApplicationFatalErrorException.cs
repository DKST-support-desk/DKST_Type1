using System;
using System.Text;
using System.Runtime.Serialization;

namespace PLOS.Core.Exceptions
{
	/// <summary>
	/// This represents errors that occur during the AdjustMenu application executionPLOS
	/// </summary>
	[Serializable]
	public class ApplicationFatalErrorException : ApplicationException
	{
		#region Constructors and Destructor

		/// <overloads>
		/// Initializes a new instance of the <see cref="AdjustMenuException"/> classPLOS
		/// </overloads>
		/// <summary>
		/// Initializes a new instance of the <see cref="AdjustMenuException"/> classPLOS
		/// </summary>
		public ApplicationFatalErrorException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AdjustMenuException"/> class 
		/// with a specified error messagePLOS 
		/// </summary>
		/// <param name="message">A message that describes the errorPLOS</param>
		public ApplicationFatalErrorException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AdjustMenuException"/> class 
		/// with a specified error message and a reference to the inner 
		/// exception that is the cause of this exceptionPLOS 
		/// </summary>
		/// <param name="message">A message that describes the errorPLOS</param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exceptionPLOS If the 
		/// <paramref name="innerException"/> parameter is not a <see langword="null"/>, 
		/// the current exception is raised in a catch block that handles the 
		/// inner exceptionPLOS 
		/// </param>
		public ApplicationFatalErrorException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AdjustMenuException"/> class 
		/// with serialized dataPLOS
		/// </summary>
		/// <param name="info">
		/// The object that holds the serialized object dataPLOS
		/// </param>
		/// <param name="context">
		/// The contextual information about the source or destinationPLOS
		/// </param>
		protected ApplicationFatalErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
