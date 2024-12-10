using System;
using CacheEditor.RTE;

namespace CacheEditor.RTE
{

	public interface IRteTarget : IEquatable<IRteTarget>
	{
		IRteProvider Provider { get; }

		object Id { get; }

		string DisplayName { get; }
	}

}
