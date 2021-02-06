using System;
using System.Collections.Generic;
using System.Linq;

namespace Clones
{
	public class CloneVersionSystem : ICloneVersionSystem
	{
		private Clones Clones { get; }

		public CloneVersionSystem()
		{
			Clones = new Clones();
		}

		public string Execute(string query)
		{
			var commandAndArgsStings = query.Split((char[])null, 2, StringSplitOptions.RemoveEmptyEntries);
			switch (commandAndArgsStings[0])
			{
				case "learn":
					var args = commandAndArgsStings[1].Split((char[])null, 2, StringSplitOptions.RemoveEmptyEntries);
					int clone = ParseIntArgument(args[0], "learn");
					int program = ParseIntArgument(args[1], "learn");
					Clones.Learn(clone, program);
					return null;
				case "rollback":
					clone = ParseIntArgument(commandAndArgsStings[1], "rollback");
					Clones.RollBack(clone);
					return null;
				case "relearn":
					clone = ParseIntArgument(commandAndArgsStings[1], "relearn");
					Clones.ReLearn(clone);
					return null;
				case "clone":
					clone = ParseIntArgument(commandAndArgsStings[1], "clone");
					Clones.Clone(clone);
					return null;
				case "check":
					clone = ParseIntArgument(commandAndArgsStings[1], "check");
					return Clones.Check(clone);
				default:
					throw new ArgumentException("Invalid command name.");
			}
		}
		public int ParseIntArgument(string arg, string commandName)
		{
			if (int.TryParse(arg, out int intArg))
				return intArg;
			throw new ArgumentException($"Can't parse argument for {commandName}.");
		}
	}

	public class Clones
	{
		List<Clone> AllClones { get; }

		public Clones()
		{
			AllClones = new List<Clone>();
			AllClones.Add(new Clone());
		}

		public void Learn(int clone, int program)
		{
			CreateNewCloneIfMoreThanOneReference(clone);
			AllClones[clone - 1].Learn(program);
		}

		public void RollBack(int clone)
		{
			CreateNewCloneIfMoreThanOneReference(clone);
			AllClones[clone - 1].RollBack();
		}

		public void ReLearn(int clone)
		{
			CreateNewCloneIfMoreThanOneReference(clone);
			AllClones[clone - 1].ReLearn();
		}

		public string Check(int clone)
		{
			return AllClones[clone - 1].Check();
		}

		public void Clone(int clone)
		{
			AllClones.Add(AllClones[clone - 1]);
			AllClones[clone - 1].AddReference();
		}

		private void CreateNewCloneIfMoreThanOneReference(int clone)
		{
			if (AllClones[clone - 1].QtyOfReferences > 1)
			{
				AllClones[clone - 1].DeleteReference();
				AllClones[clone - 1] = AllClones[clone - 1].CopyThisClone();
			}
		}
	}
	public class Clone
	{
		public int QtyOfReferences { get; protected set; }

		protected Stack<int> Programs { get; }

		protected Stack<int> RollBacks { get; }

		public Clone()
		{
			Programs = new Stack<int>();
			RollBacks = new Stack<int>();
			QtyOfReferences = 1;
		}

		public Clone(IEnumerable<int> programs, IEnumerable<int> rollBacks)
		{
			Programs = new Stack<int>(programs);
			RollBacks = new Stack<int>(rollBacks);
			QtyOfReferences = 1;
		}

		public void AddReference() => QtyOfReferences++;
		public void DeleteReference() => QtyOfReferences--;

		public void Learn(int program)
		{
			Programs.Push(program);
			RollBacks.Clear();
		}

		public void RollBack()
		{
			RollBacks.Push(Programs.Pop());
		}

		public void ReLearn()
		{
			Programs.Push(RollBacks.Pop());
		}

		public string Check()
		{
			if (Programs.Count > 0)
				return Programs.Peek().ToString();
			return "basic";
		}

		public Clone CopyThisClone()
		{
			return new Clone(Programs.Reverse(), RollBacks.Reverse());
		}
	}
}
