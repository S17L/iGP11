using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace iGP11.Library
{
    public sealed class MappingExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _expression;
        private readonly Expression _mappingExpression;

        private bool? _areEqual;
        private Queue<Expression> _expressionExpressions;
        private Queue<Expression> _mappingPartialExpressions;

        public MappingExpressionVisitor(Expression mappingExpression, Expression expression)
        {
            if (mappingExpression == null)
            {
                throw new ArgumentException(nameof(mappingExpression));
            }

            if (expression == null)
            {
                throw new ArgumentException(nameof(expression));
            }

            _mappingExpression = mappingExpression;
            _expression = expression;
        }

        public bool AreEqual
        {
            get
            {
                if (!_areEqual.HasValue)
                {
                    _areEqual = true;
                    _expressionExpressions = new Queue<Expression>(new ExpressionExtractor().Extract(_expression));
                    _mappingPartialExpressions = new Queue<Expression>(new ExpressionExtractor
                    {
                        EnsureUniqueness = IsPartialExpressionCheckingEnabled
                    }.Extract(_mappingExpression));

                    Visit(_expression);
                    if (HasAnyMappingPartialExpressions)
                    {
                        MarkAsNotEqual();
                    }
                }

                return _areEqual.GetValueOrDefault();
            }
        }

        public bool IsPartialExpressionCheckingEnabled { get; set; }

        private bool HasAnyMappingPartialExpressions => _mappingPartialExpressions.Any();

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (!AreEqual)
            {
                return base.VisitBinary(node);
            }

            var partialMappingExpression = PeekNextPartialMappingExpression<BinaryExpression>();
            if (partialMappingExpression != null)
            {
                DequeueNextPartialMappingExpression<BinaryExpression>();
                if (!CheckIfAreEqual(partialMappingExpression.Method, node.Method) ||
                    !CheckIfAreEqual(partialMappingExpression.IsLifted, node.IsLifted) ||
                    !CheckIfAreEqual(partialMappingExpression.IsLiftedToNull, node.IsLiftedToNull))
                {
                    MarkAsNotEqual();
                }
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            throw new NotSupportedException();
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (!AreEqual)
            {
                return base.VisitConstant(node);
            }

            if (IsPartialExpressionCheckingEnabled && (PeekNextPartialMappingExpression<ConstantExpression>() == null))
            {
                return base.VisitConstant(node);
            }

            var partialMappingExpression = DequeueNextPartialMappingExpression<ConstantExpression>();
            if (partialMappingExpression == null)
            {
                MarkAsNotEqual();
            }
            else if (!CheckIfAreEqual(partialMappingExpression.Value, node.Value))
            {
                MarkAsNotEqual();
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            throw new NotSupportedException();
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitExtension(Expression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            throw new NotSupportedException();
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (!AreEqual)
            {
                return base.VisitLambda(node);
            }

            var partialMappingExpression = DequeueNextPartialMappingExpression<Expression>();
            if (IsPartialExpressionCheckingEnabled)
            {
                return base.VisitLambda(node);
            }

            if (partialMappingExpression == null)
            {
                MarkAsNotEqual();
            }
            else if (!CheckIfAreEqual(partialMappingExpression.Type, node.Type) && !CheckIfAreOfSameType(partialMappingExpression.Type, node.Type))
            {
                MarkAsNotEqual();
            }

            return base.VisitLambda(node);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (!AreEqual)
            {
                return base.VisitMember(node);
            }

            if (IsPartialExpressionCheckingEnabled && (PeekNextPartialMappingExpression<MemberExpression>() == null))
            {
                if (!HasAnyMappingPartialExpressions)
                {
                    return base.VisitMember(node);
                }

                var parameterMappingExpression = DequeueNextPartialMappingExpression<ParameterExpression>();
                if (HasAnyMappingPartialExpressions || (parameterMappingExpression == null))
                {
                    MarkAsNotEqual();
                }
                else if (!CheckIfAreEqual(parameterMappingExpression.Type, node.Type) && !CheckIfAreOfSameType(parameterMappingExpression.Type, node.Type, false))
                {
                    MarkAsNotEqual();
                }

                return base.VisitMember(node);
            }

            var partialMappingExpression = DequeueNextPartialMappingExpression<MemberExpression>();
            if (partialMappingExpression == null)
            {
                MarkAsNotEqual();
            }
            else if (!CheckIfAreEqual(partialMappingExpression.Member, node.Member) && !CheckIfIsAssignableFrom(partialMappingExpression.Member, node.Member))
            {
                MarkAsNotEqual();
            }

            return base.VisitMember(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            throw new NotSupportedException();
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            throw new NotSupportedException();
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            throw new NotSupportedException();
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!AreEqual)
            {
                return base.VisitMethodCall(node);
            }

            if (IsPartialExpressionCheckingEnabled && (PeekNextPartialMappingExpression<MethodCallExpression>() == null))
            {
                return base.VisitMethodCall(node);
            }

            var partialMappingExpression = DequeueNextPartialMappingExpression<MethodCallExpression>();
            if (partialMappingExpression == null)
            {
                MarkAsNotEqual();
            }
            else if (!CheckIfAreEqual(partialMappingExpression.Method, node.Method) && !CheckIfIsAssignableFrom(partialMappingExpression.Method, node.Method))
            {
                MarkAsNotEqual();
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!AreEqual)
            {
                return base.VisitParameter(node);
            }

            if (IsPartialExpressionCheckingEnabled && (PeekNextPartialMappingExpression<ParameterExpression>() == null))
            {
                return base.VisitParameter(node);
            }

            var partialMappingExpression = DequeueNextPartialMappingExpression<ParameterExpression>();
            if (partialMappingExpression == null)
            {
                MarkAsNotEqual();
            }
            else if (!CheckIfAreEqual(partialMappingExpression.Type, node.Type) && !CheckIfAreOfSameType(partialMappingExpression.Type, node.Type, false))
            {
                MarkAsNotEqual();
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            throw new NotSupportedException();
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitTry(TryExpression node)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (!AreEqual)
            {
                return base.VisitTypeBinary(node);
            }

            if (IsPartialExpressionCheckingEnabled && (PeekNextPartialMappingExpression<TypeBinaryExpression>() == null))
            {
                return base.VisitTypeBinary(node);
            }

            var partialMappingExpression = PeekNextPartialMappingExpression<TypeBinaryExpression>();
            if (partialMappingExpression != null)
            {
                DequeueNextPartialMappingExpression<TypeBinaryExpression>();
                if (!CheckIfAreEqual(partialMappingExpression.TypeOperand, node.TypeOperand))
                {
                    MarkAsNotEqual();
                }
            }

            return base.VisitTypeBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (!AreEqual)
            {
                return base.VisitUnary(node);
            }

            if (IsPartialExpressionCheckingEnabled && (PeekNextPartialMappingExpression<TypeBinaryExpression>() == null))
            {
                return base.VisitUnary(node);
            }

            var partialMappingExpression = PeekNextPartialMappingExpression<UnaryExpression>();
            if (partialMappingExpression != null)
            {
                DequeueNextPartialMappingExpression<UnaryExpression>();
                if (!CheckIfAreEqual(partialMappingExpression.Method, node.Method) ||
                    !CheckIfAreEqual(partialMappingExpression.IsLifted, node.IsLifted) ||
                    !CheckIfAreEqual(partialMappingExpression.IsLiftedToNull, node.IsLiftedToNull))
                {
                    MarkAsNotEqual();
                }
            }

            return base.VisitUnary(node);
        }

        private static bool CheckIfAreEqual<TObject>(TObject first, TObject second)
        {
            return EqualityComparer<TObject>.Default.Equals(first, second);
        }

        private static bool CheckIfAreOfSameType(Type first, Type second, bool isNameCheckingEnabled = true)
        {
            return CheckIfIsAssignableFromType(first, second) && (!isNameCheckingEnabled || (first.Name == second.Name));
        }

        private static bool CheckIfIsAssignableFrom<TMemberInfo>(TMemberInfo first, TMemberInfo second)
            where TMemberInfo : MemberInfo
        {
            return CheckIfIsAssignableFromType(first.DeclaringType, second.DeclaringType) && (first.Name == second.Name);
        }

        private static bool CheckIfIsAssignableFromType(Type first, Type second)
        {
            return first.IsAssignableFrom(second) || second.IsAssignableFrom(first);
        }

        private void DequeueNextExpression()
        {
            if (_expressionExpressions.Any())
            {
                _expressionExpressions.Dequeue();
            }
        }

        private TExpression DequeueNextPartialMappingExpression<TExpression>()
            where TExpression : Expression
        {
            DequeueNextExpression();
            return _mappingPartialExpressions.Any()
                       ? _mappingPartialExpressions.Dequeue() as TExpression
                       : null;
        }

        private void MarkAsNotEqual()
        {
            _areEqual = false;
        }

        private TExpression PeekNextPartialMappingExpression<TExpression>()
            where TExpression : Expression
        {
            return _mappingPartialExpressions.Any()
                       ? _mappingPartialExpressions.Peek() as TExpression
                       : null;
        }
    }
}