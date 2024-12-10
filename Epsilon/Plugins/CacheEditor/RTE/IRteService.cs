using System.Collections.Generic;
using CacheEditor;
using CacheEditor.RTE;

namespace CacheEditor.RTE
{

	public interface IRteService
	{
		IEnumerable<IRteProvider> Providers { get; }
		IRteTargetCollection GetTargetList(ICacheFile cacheFile);
	}

}
