using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

// The integer type used to represent a handle
using HandleWord = System.Int32;
// The unsigned equivalent of HandleWord's underlying type
using HandleWordUnsigned = System.UInt32;

namespace PhxLib
{
	partial class Util
	{
		const HandleWordUnsigned kUndefinedReferenceHandleBitmask = 
			unchecked((HandleWordUnsigned)HandleWord.MinValue); // 0x80...

		public static bool IsUndefinedReferenceHandle(HandleWord handle)
		{
			HandleWordUnsigned uhandle = (HandleWordUnsigned)handle;

			return (uhandle & kUndefinedReferenceHandleBitmask) != 0;
		}
		public static HandleWord GetUndefinedReferenceDataIndex(HandleWord undefined_ref_handle)
		{
			HandleWordUnsigned uhandle = (HandleWordUnsigned)undefined_ref_handle;

			return (HandleWord)(uhandle & ~kUndefinedReferenceHandleBitmask);
		}
		public static HandleWord GetUndefinedReferenceHandle(HandleWord undefined_ref_data_index)
		{
			Contract.Requires(undefined_ref_data_index < HandleWord.MaxValue,
				"Index value would generate a handle that matches the general invalid-handle sentinel");

			HandleWordUnsigned index = (HandleWordUnsigned)undefined_ref_data_index;

			return (HandleWord)(index | kUndefinedReferenceHandleBitmask);
		}
	};
};