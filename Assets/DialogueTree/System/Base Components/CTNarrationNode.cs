#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Components
{
    using Data.Save;
    using Enums;
    using Utils;
    using GraphView;

    public class CTNarrationNode : CTNode
    {
        public override void Initialise(string _name, CTGraphView _view, Vector2 _pos)
        {
            base.Initialise(_name, _view, _pos);

            node_type = CTNodeType.Narration;

            CTOptionSaveData option_data = new CTOptionSaveData();
            option_data.text = "Next Node";

            options.Add(option_data);
        }

        public override void Draw()
        {
            base.Draw();

            foreach (CTOptionSaveData o in options)
            {
                Port out_port = this.CreatePort(o.text);

                out_port.userData = o;

                outputContainer.Add(out_port);
            }

            RefreshExpandedState();
        }
    }
}

#endif