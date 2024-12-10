﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using CacheEditor;
using CacheEditor.RTE;

namespace CacheEditor.RTE
{
	[Export(typeof(IRteService))]
	internal class RteService : IRteService
	{
		public IEnumerable<IRteProvider> Providers { get; }

		[ImportingConstructor]
		public RteService([ImportMany] IEnumerable<IRteProvider> providers) {
			Providers = providers;
		}

		public IRteTargetCollection GetTargetList(ICacheFile cacheFile) {
			AggregateTargetSource source = new AggregateTargetSource();
			foreach (IRteProvider provider in Providers) {
				if (provider.ValidForCacheFile(cacheFile)) {
					source.Add(provider);
				}
			}
			return new RteTargetCollection(source);
		}
	}

}
