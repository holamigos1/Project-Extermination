using System;
using System.Collections.Generic;
using Scripts.GameEnums;
using UnityEngine;

namespace Scripts.GameEnums
{
    public enum MouseScroll
    {
        Up,
        Down
    }
}

namespace Scripts.Controllers
{
    public class InputHandlersController : MonoBehaviour
    {
        public Action<KeyCode[]> ControlKeysHolded = delegate { };
        public Action<KeyCode> KeyHolded = delegate { };
        public Action<KeyCode> KeyReleased = delegate { };
        public Action<KeyCode> KeyTapped = delegate { };
        public Action<float, float> MouseMoved = delegate { };
        public Action<MouseScroll> MouseScrolling = delegate { };

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) KeyTapped(KeyCode.Mouse0);
            if (Input.GetKeyDown(KeyCode.Mouse1)) KeyTapped(KeyCode.Mouse1);
            if (Input.GetKeyDown(KeyCode.Mouse2)) KeyTapped(KeyCode.Mouse2);

            if (Input.GetKeyDown(KeyCode.Q)) KeyTapped(KeyCode.Q);
            if (Input.GetKeyDown(KeyCode.W)) KeyTapped(KeyCode.W);
            if (Input.GetKeyDown(KeyCode.E)) KeyTapped(KeyCode.E);
            if (Input.GetKeyDown(KeyCode.R)) KeyTapped(KeyCode.R);
            if (Input.GetKeyDown(KeyCode.T)) KeyTapped(KeyCode.T);
            if (Input.GetKeyDown(KeyCode.Y)) KeyTapped(KeyCode.Y);
            if (Input.GetKeyDown(KeyCode.U)) KeyTapped(KeyCode.U);
            if (Input.GetKeyDown(KeyCode.I)) KeyTapped(KeyCode.I);
            if (Input.GetKeyDown(KeyCode.O)) KeyTapped(KeyCode.O);
            if (Input.GetKeyDown(KeyCode.P)) KeyTapped(KeyCode.P);
            if (Input.GetKeyDown(KeyCode.LeftBracket)) KeyTapped(KeyCode.LeftBracket);
            if (Input.GetKeyDown(KeyCode.RightBracket)) KeyTapped(KeyCode.RightBracket);

            if (Input.GetKeyDown(KeyCode.A)) KeyTapped(KeyCode.A);
            if (Input.GetKeyDown(KeyCode.S)) KeyTapped(KeyCode.S);
            if (Input.GetKeyDown(KeyCode.D)) KeyTapped(KeyCode.D);
            if (Input.GetKeyDown(KeyCode.F)) KeyTapped(KeyCode.F);
            if (Input.GetKeyDown(KeyCode.G)) KeyTapped(KeyCode.G);
            if (Input.GetKeyDown(KeyCode.H)) KeyTapped(KeyCode.H);
            if (Input.GetKeyDown(KeyCode.J)) KeyTapped(KeyCode.J);
            if (Input.GetKeyDown(KeyCode.K)) KeyTapped(KeyCode.K);
            if (Input.GetKeyDown(KeyCode.L)) KeyTapped(KeyCode.L);
            if (Input.GetKeyDown(KeyCode.Semicolon)) KeyTapped(KeyCode.Semicolon);
            if (Input.GetKeyDown(KeyCode.DoubleQuote)) KeyTapped(KeyCode.DoubleQuote);

            if (Input.GetKeyDown(KeyCode.Z)) KeyTapped(KeyCode.Z);
            if (Input.GetKeyDown(KeyCode.X)) KeyTapped(KeyCode.X);
            if (Input.GetKeyDown(KeyCode.C)) KeyTapped(KeyCode.C);
            if (Input.GetKeyDown(KeyCode.V)) KeyTapped(KeyCode.V);
            if (Input.GetKeyDown(KeyCode.B)) KeyTapped(KeyCode.B);
            if (Input.GetKeyDown(KeyCode.N)) KeyTapped(KeyCode.N);
            if (Input.GetKeyDown(KeyCode.M)) KeyTapped(KeyCode.M);
            if (Input.GetKeyDown(KeyCode.Comma)) KeyTapped(KeyCode.Comma);
            if (Input.GetKeyDown(KeyCode.Period)) KeyTapped(KeyCode.Period);
            if (Input.GetKeyDown(KeyCode.Slash)) KeyTapped(KeyCode.Slash);

            if (Input.GetKeyDown(KeyCode.LeftShift)) KeyTapped(KeyCode.LeftShift);
            if (Input.GetKeyDown(KeyCode.LeftControl)) KeyTapped(KeyCode.LeftControl);
            if (Input.GetKeyDown(KeyCode.LeftAlt)) KeyTapped(KeyCode.LeftAlt);
            if (Input.GetKeyDown(KeyCode.Space)) KeyTapped(KeyCode.Space);

            ////////////////////////////////////////////////////////////////////////////////////

            var pressedKeys = new List<KeyCode>();

            if (Input.GetKey(KeyCode.W))
            {
                KeyHolded(KeyCode.W);
                pressedKeys.Add(KeyCode.W);
            }

            if (Input.GetKey(KeyCode.A))
            {
                KeyHolded(KeyCode.A);
                pressedKeys.Add(KeyCode.A);
            }

            if (Input.GetKey(KeyCode.S))
            {
                KeyHolded(KeyCode.S);
                pressedKeys.Add(KeyCode.S);
            }

            if (Input.GetKey(KeyCode.D))
            {
                KeyHolded(KeyCode.D);
                pressedKeys.Add(KeyCode.D);
            }

            ControlKeysHolded(pressedKeys.ToArray());

            ////////////////////////////////////////////////////////////////////////////////////
            if (Input.GetKey(KeyCode.Mouse0)) KeyHolded(KeyCode.Mouse0);
            if (Input.GetKey(KeyCode.Mouse1)) KeyHolded(KeyCode.Mouse1);
            if (Input.GetKey(KeyCode.Mouse2)) KeyHolded(KeyCode.Mouse2);


            if (Input.GetKey(KeyCode.Q)) KeyHolded(KeyCode.Q);
            if (Input.GetKey(KeyCode.E)) KeyHolded(KeyCode.E);
            if (Input.GetKey(KeyCode.R)) KeyHolded(KeyCode.R);
            if (Input.GetKey(KeyCode.T)) KeyHolded(KeyCode.T);
            if (Input.GetKey(KeyCode.Y)) KeyHolded(KeyCode.Y);
            if (Input.GetKey(KeyCode.U)) KeyHolded(KeyCode.U);
            if (Input.GetKey(KeyCode.I)) KeyHolded(KeyCode.I);
            if (Input.GetKey(KeyCode.O)) KeyHolded(KeyCode.O);
            if (Input.GetKey(KeyCode.P)) KeyHolded(KeyCode.P);
            if (Input.GetKey(KeyCode.LeftBracket)) KeyHolded(KeyCode.LeftBracket);
            if (Input.GetKey(KeyCode.RightBracket)) KeyHolded(KeyCode.RightBracket);

            if (Input.GetKey(KeyCode.F)) KeyHolded(KeyCode.F);
            if (Input.GetKey(KeyCode.G)) KeyHolded(KeyCode.G);
            if (Input.GetKey(KeyCode.H)) KeyHolded(KeyCode.H);
            if (Input.GetKey(KeyCode.J)) KeyHolded(KeyCode.J);
            if (Input.GetKey(KeyCode.K)) KeyHolded(KeyCode.K);
            if (Input.GetKey(KeyCode.L)) KeyHolded(KeyCode.L);
            if (Input.GetKey(KeyCode.Semicolon)) KeyHolded(KeyCode.Semicolon);
            if (Input.GetKey(KeyCode.DoubleQuote)) KeyHolded(KeyCode.DoubleQuote);

            if (Input.GetKey(KeyCode.Z)) KeyHolded(KeyCode.Z);
            if (Input.GetKey(KeyCode.X)) KeyHolded(KeyCode.X);
            if (Input.GetKey(KeyCode.C)) KeyHolded(KeyCode.C);
            if (Input.GetKey(KeyCode.V)) KeyHolded(KeyCode.V);
            if (Input.GetKey(KeyCode.B)) KeyHolded(KeyCode.B);
            if (Input.GetKey(KeyCode.N)) KeyHolded(KeyCode.N);
            if (Input.GetKey(KeyCode.M)) KeyHolded(KeyCode.M);
            if (Input.GetKey(KeyCode.Comma)) KeyHolded(KeyCode.Comma);
            if (Input.GetKey(KeyCode.Period)) KeyHolded(KeyCode.Period);
            if (Input.GetKey(KeyCode.Slash)) KeyHolded(KeyCode.Slash);

            if (Input.GetKey(KeyCode.LeftShift)) KeyHolded(KeyCode.LeftShift);
            if (Input.GetKey(KeyCode.LeftControl)) KeyHolded(KeyCode.LeftControl);
            if (Input.GetKey(KeyCode.LeftAlt)) KeyHolded(KeyCode.LeftAlt);
            if (Input.GetKey(KeyCode.Space)) KeyHolded(KeyCode.Space);

            ////////////////////////////////////////////////////////////////////////////////////
            if (Input.GetKeyUp(KeyCode.Mouse0)) KeyReleased(KeyCode.Mouse0);
            if (Input.GetKeyUp(KeyCode.Mouse1)) KeyReleased(KeyCode.Mouse1);
            if (Input.GetKeyUp(KeyCode.Mouse2)) KeyReleased(KeyCode.Mouse2);

            if (Input.GetKeyUp(KeyCode.Q)) KeyReleased(KeyCode.Q);
            if (Input.GetKeyUp(KeyCode.W)) KeyReleased(KeyCode.W);
            if (Input.GetKeyUp(KeyCode.E)) KeyReleased(KeyCode.E);
            if (Input.GetKeyUp(KeyCode.R)) KeyReleased(KeyCode.R);
            if (Input.GetKeyUp(KeyCode.T)) KeyReleased(KeyCode.T);
            if (Input.GetKeyUp(KeyCode.Y)) KeyReleased(KeyCode.Y);
            if (Input.GetKeyUp(KeyCode.U)) KeyReleased(KeyCode.U);
            if (Input.GetKeyUp(KeyCode.I)) KeyReleased(KeyCode.I);
            if (Input.GetKeyUp(KeyCode.O)) KeyReleased(KeyCode.O);
            if (Input.GetKeyUp(KeyCode.P)) KeyReleased(KeyCode.P);
            if (Input.GetKeyUp(KeyCode.LeftBracket)) KeyReleased(KeyCode.LeftBracket);
            if (Input.GetKeyUp(KeyCode.RightBracket)) KeyReleased(KeyCode.RightBracket);

            if (Input.GetKeyUp(KeyCode.A)) KeyReleased(KeyCode.A);
            if (Input.GetKeyUp(KeyCode.S)) KeyReleased(KeyCode.S);
            if (Input.GetKeyUp(KeyCode.D)) KeyReleased(KeyCode.D);
            if (Input.GetKeyUp(KeyCode.F)) KeyReleased(KeyCode.F);
            if (Input.GetKeyUp(KeyCode.G)) KeyReleased(KeyCode.G);
            if (Input.GetKeyUp(KeyCode.H)) KeyReleased(KeyCode.H);
            if (Input.GetKeyUp(KeyCode.J)) KeyReleased(KeyCode.J);
            if (Input.GetKeyUp(KeyCode.K)) KeyReleased(KeyCode.K);
            if (Input.GetKeyUp(KeyCode.L)) KeyReleased(KeyCode.L);
            if (Input.GetKeyUp(KeyCode.Semicolon)) KeyReleased(KeyCode.Semicolon);
            if (Input.GetKeyUp(KeyCode.DoubleQuote)) KeyReleased(KeyCode.DoubleQuote);

            if (Input.GetKeyUp(KeyCode.Z)) KeyReleased(KeyCode.Z);
            if (Input.GetKeyUp(KeyCode.X)) KeyReleased(KeyCode.X);
            if (Input.GetKeyUp(KeyCode.C)) KeyReleased(KeyCode.C);
            if (Input.GetKeyUp(KeyCode.V)) KeyReleased(KeyCode.V);
            if (Input.GetKeyUp(KeyCode.B)) KeyReleased(KeyCode.B);
            if (Input.GetKeyUp(KeyCode.N)) KeyReleased(KeyCode.N);
            if (Input.GetKeyUp(KeyCode.M)) KeyReleased(KeyCode.M);
            if (Input.GetKeyUp(KeyCode.Comma)) KeyReleased(KeyCode.Comma);
            if (Input.GetKeyUp(KeyCode.Period)) KeyReleased(KeyCode.Period);
            if (Input.GetKeyUp(KeyCode.Slash)) KeyReleased(KeyCode.Slash);

            if (Input.GetKeyUp(KeyCode.LeftShift)) KeyReleased(KeyCode.LeftShift);
            if (Input.GetKeyUp(KeyCode.LeftControl)) KeyReleased(KeyCode.LeftControl);
            if (Input.GetKeyUp(KeyCode.LeftAlt)) KeyReleased(KeyCode.LeftAlt);
            if (Input.GetKeyUp(KeyCode.Space)) KeyReleased(KeyCode.Space);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (Input.mouseScrollDelta.y != 0)
            {
                if (Input.mouseScrollDelta.y > 0) MouseScrolling(MouseScroll.Up);
                if (Input.mouseScrollDelta.y < 0) MouseScrolling(MouseScroll.Down);
            }

            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                MouseMoved(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }
}