﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CodeRefactorings
{
    internal interface IRefactoringHelpersService : ILanguageService
    {
        /// <summary>
        /// <para>
        /// Returns an instance of <typeparamref name="TSyntaxNode"/> for refactoring given specified selection in document or null
        /// if no such instance exists.
        /// </para>
        /// <para>
        /// A <typeparamref name="TSyntaxNode"/> instance is returned if:
        /// - Selection is zero-width and inside/touching a Token with direct parent of type <typeparamref name="TSyntaxNode"/>.
        /// - Selection is zero-width and touching a Token whose ancestor ends/starts precisely on current selection .
        /// - Token whose direct parent of type <typeparamref name="TSyntaxNode"/> is selected.
        /// - Whole node of a type <typeparamref name="TSyntaxNode"/> is selected.
        /// </para>
        /// <para>
        /// Note: this function strips all whitespace from both the beginning and the end of given <paramref name="selection"/>.
        /// The stripped version is then used to determine relevant <see cref="SyntaxNode"/>. It also handles incomplete selections
        /// of tokens gracefully.
        /// </para>
        /// </summary>
        Task<SyntaxNode> TryGetSelectedNodeAsync(Document document, TextSpan selection, Func<SyntaxNode, SyntaxNode> extractNode, Predicate<SyntaxNode> predicate, CancellationToken cancellationToken);
        Task<TSyntaxNode> TryGetSelectedNodeAsync<TSyntaxNode>(Document document, TextSpan selection, CancellationToken cancellationToken) where TSyntaxNode : SyntaxNode;
        Task<TSyntaxNode> TryGetSelectedNodeAsync<TSyntaxNode>(Document document, TextSpan selection, Func<SyntaxNode, SyntaxNode> extractNode, CancellationToken cancellationToken) where TSyntaxNode : SyntaxNode;
        SyntaxNode ExtractNodeFromDeclarationAndAssignment<TNode>(SyntaxNode current) where TNode : SyntaxNode;
    }
}
