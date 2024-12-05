﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpsilonLib.Editors
{
    public interface IEditorService
    {
        IEnumerable<IEditorProvider> EditorProviders { get; }

        Task OpenFileWithEditorAsync(Guid editorProviderId, params string[] paths);
        Task OpenFileAsync(string filePath);
    }
}
