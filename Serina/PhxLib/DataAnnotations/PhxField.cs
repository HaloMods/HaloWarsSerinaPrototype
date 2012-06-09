using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhxLib.DataAnnotations
{
	/// <summary>How the backing field for a property is implemented</summary>
	public enum PropertyBackingType
	{
		/// <summary>.NET's implementation of auto properties</summary>
		AutoProperty,
		/// <summary>Backing field is the name of the property prefixed with an 'm'</summary>
		Ensemble,
	};

	public enum PropertyInvalidValueType
	{
		MinusOne = -1,
		Zero,
		NaN,

		Null = Zero,
	};

	// http://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression

	class PhxField
	{
	}
}