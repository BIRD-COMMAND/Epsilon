using System.Collections.Generic;
using CacheEditor.RTE;

namespace CacheEditor.RTE
{
	public interface IRteTargetSource
	{
		IEnumerable<IRteTarget> FindTargets();
	}
}
