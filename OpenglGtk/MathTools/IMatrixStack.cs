using System;
using System.Collections.Generic;

using System.Text;

namespace MathTools
{
    public class IMatrixStack
    {
        Stack<IMatrix> stack;

        public IMatrixStack(){
            stack = new Stack<IMatrix>();
        }

        public void SetIdentity()
        {
            stack.Clear();
            stack.Push(IMatrix.Identity());
        }

        public void Push()
        {
            stack.Push(Top());
        }

        public void MultMatrix(IMatrix im)
        {
            stack.Push(Top().Dot(im));
        }

		public IMatrix Pop()
        {
            return stack.Pop();
        }

		public IMatrix Top()
        {
			return new IMatrix(stack.Peek().Direct,stack.Peek().Inverse);
        }

    }
}
