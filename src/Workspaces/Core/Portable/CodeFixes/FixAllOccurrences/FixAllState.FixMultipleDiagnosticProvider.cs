﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CodeFixes
{
    internal partial class FixAllState
    {
        /// <summary>
        /// Diagnostic provider to fetch document/project diagnostics to fix in a <see cref="FixAllContext"/>.
        /// </summary>
        internal sealed class FixMultipleDiagnosticProvider : FixAllContext.DiagnosticProvider
        {
            internal override bool IsFixMultiple => true;

            private readonly ImmutableDictionary<Document, ImmutableArray<Diagnostic>> _documentDiagnosticsMap;
            private readonly ImmutableDictionary<Project, ImmutableArray<Diagnostic>> _projectDiagnosticsMap;

            public FixMultipleDiagnosticProvider(ImmutableDictionary<Document, ImmutableArray<Diagnostic>> diagnosticsMap)
            {
                _documentDiagnosticsMap = diagnosticsMap;
                _projectDiagnosticsMap = ImmutableDictionary<Project, ImmutableArray<Diagnostic>>.Empty;
            }

            public FixMultipleDiagnosticProvider(ImmutableDictionary<Project, ImmutableArray<Diagnostic>> diagnosticsMap)
            {
                _projectDiagnosticsMap = diagnosticsMap;
                _documentDiagnosticsMap = ImmutableDictionary<Document, ImmutableArray<Diagnostic>>.Empty;
            }

            internal override Task<ImmutableDictionary<Document, ImmutableArray<Diagnostic>>> GetDocumentDiagnosticsToFixWorkerAsync(
                FixAllContext context)
            {
                return Task.FromResult(_documentDiagnosticsMap);
            }

            internal override Task<ImmutableDictionary<Project, ImmutableArray<Diagnostic>>> GetProjectDiagnosticsToFixAsync(
                FixAllContext context)
            {
                return Task.FromResult(_projectDiagnosticsMap);
            }

            public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                var allDiagnosticsBuilder = ArrayBuilder<Diagnostic>.GetInstance();
                ImmutableArray<Diagnostic> diagnostics;
                if (!_documentDiagnosticsMap.IsEmpty)
                {
                    foreach (var document in project.Documents)
                    {
                        if (_documentDiagnosticsMap.TryGetValue(document, out diagnostics))
                        {
                            allDiagnosticsBuilder.AddRange(diagnostics);
                        }
                    }
                }

                if (_projectDiagnosticsMap.TryGetValue(project, out diagnostics))
                {
                    allDiagnosticsBuilder.AddRange(diagnostics);
                }

                return Task.FromResult<IEnumerable<Diagnostic>>(allDiagnosticsBuilder.ToImmutableAndFree());
            }

            public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
            {
                if (_documentDiagnosticsMap.TryGetValue(document, out var diagnostics))
                {
                    return Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);
                }

                return Task.FromResult(SpecializedCollections.EmptyEnumerable<Diagnostic>());
            }

            public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                if (_projectDiagnosticsMap.TryGetValue(project, out var diagnostics))
                {
                    return Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);
                }

                return Task.FromResult(SpecializedCollections.EmptyEnumerable<Diagnostic>());
            }
        }
    }
}
