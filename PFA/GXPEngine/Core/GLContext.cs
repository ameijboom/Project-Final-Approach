//#define USE_FMOD_AUDIO
#define STRETCH_ON_RESIZE

using System;
using System.IO;
using Arqan;
using System.Text;
using System.Runtime.InteropServices;
using GXPEngine.Core;
using System.Linq;

namespace GXPEngine.Core
{

    class WindowSize
    {
        public static WindowSize instance = new WindowSize();
        public int width, height;
    }

    public class GLContext
    {

        const int MAXKEYS = 65535;
        const int MAXBUTTONS = 255;

        private static bool[] keys = new bool[MAXKEYS + 1];
        private static bool[] keydown = new bool[MAXKEYS + 1];
        private static bool[] keyup = new bool[MAXKEYS + 1];
        private static bool[] buttons = new bool[MAXBUTTONS + 1];
        private static bool[] mousehits = new bool[MAXBUTTONS + 1];
        private static bool[] mouseup = new bool[MAXBUTTONS + 1]; //mouseup kindly donated by LeonB
        private static int keyPressedCount = 0;
        private static bool anyKeyDown = false;

        public static double mouseX = 0;
        public static double mouseY = 0;

        private Game _owner;
        private static SoundSystem _soundSystem;

        private int _targetFrameRate = 60;
        private long _lastFrameTime = 0;
        private long _lastFPSTime = 0;
        private int _frameCount = 0;
        private int _lastFPS = 0;
        private bool _vsyncEnabled = false;

        private static double _realToLogicWidthRatio;
        private static double _realToLogicHeightRatio;

        public static IntPtr Window;

        private GLData _data;

        private uint vertexShader;
        private uint fragmentShader;
        private uint shaderProgram;

        private static readonly uint[] indices = {
                0, 1, 3, // first triangle
                1, 2, 3  // second triangle
        };

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public GLContext(Game owner)
        {
            _owner = owner;
            _lastFPS = _targetFrameRate;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Width
        //------------------------------------------------------------------------------------------------------------------------
        public int width
        {
            get { return WindowSize.instance.width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Height
        //------------------------------------------------------------------------------------------------------------------------
        public int height
        {
            get { return WindowSize.instance.height; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SoundSystem
        //------------------------------------------------------------------------------------------------------------------------
        public static SoundSystem soundSystem
        {
            get
            {
                if (_soundSystem == null)
                {
                    InitializeSoundSystem();
                }
                return _soundSystem;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														setupWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public void CreateWindow(int width, int height, bool fullScreen, bool vSync, int realWidth, int realHeight)
        {
            // This stores the "logical" width, used by all the game logic:
            WindowSize.instance.width = width;
            WindowSize.instance.height = height;
            _realToLogicWidthRatio = (double)realWidth / width;
            _realToLogicHeightRatio = (double)realHeight / height;
            _vsyncEnabled = vSync;

            GLFW.glfwInit();

            // GLFW.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            GLFW.glfwWindowHint(GLFW.GLFW_SAMPLES, 8);
            Window = GLFW.glfwCreateWindow(realWidth, realHeight, Encoding.ASCII.GetBytes("Game"), (fullScreen ? GLFW.glfwGetPrimaryMonitor() : (IntPtr)null), (IntPtr)null);

            GLFW.glfwMakeContextCurrent(Window);

            GLFW.glfwSwapInterval(vSync ? 1 : 0);

            GLFW.glfwSetKeyCallback(Window,
                (System.IntPtr _window, int _key, int _scanCode, int _action, int _mods) =>
                {
                    bool press = (_action == GLFW.GLFW_PRESS);

                    if (press)
                    {
                        keydown[((int)_key)] = true;
                        anyKeyDown = true;
                        keyPressedCount++;
                    }
                    else
                    {
                        keyup[((int)_key)] = true;
                        keyPressedCount--;
                    }

                    keys[((int)_key)] = (_action == GLFW.GLFW_REPEAT || press);
                });

            GLFW.glfwSetMouseButtonCallback(Window,
                (System.IntPtr _window, int _button, int _action, int _mods) =>
                {
                    bool press = (_action == GLFW.GLFW_PRESS);

                    if (press)
                        mousehits[((int)_button)] = true;
                    else
                        mouseup[((int)_button)] = true;

                    buttons[((int)_button)] = press;
                });

            GLFW.glfwSetWindowSizeCallback(Window, (System.IntPtr _window, int newWidth, int newHeight) =>
            {
                GL.glViewport(0, 0, newWidth, newHeight);
                GL.glEnable(GL.GL_MULTISAMPLE);
                GL.glEnable(GL.GL_TEXTURE_2D);
                GL.glEnable(GL.GL_BLEND);
                GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
                GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_FASTEST);
                GL.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);

                GL.glMatrixMode(GL.GL_PROJECTION);
                GL.glLoadIdentity();

#if STRETCH_ON_RESIZE
                _realToLogicWidthRatio = (double)newWidth / WindowSize.instance.width;
                _realToLogicHeightRatio = (double)newHeight / WindowSize.instance.height;
#endif

                GL.glOrtho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);
#if !STRETCH_ON_RESIZE
				lock (WindowSize.instance) {
					WindowSize.instance.width = (int)(newWidth/_realToLogicWidthRatio);
					WindowSize.instance.height = (int)(newHeight/_realToLogicHeightRatio);
				}
#endif

                if (Game.main != null)
                {
                    Game.main.RenderRange = new Rectangle(0, 0, WindowSize.instance.width, WindowSize.instance.height);
                }
            });
            InitializeSoundSystem();
            InitializeGLData();
			InitializeShaders();
        }

        private void InitializeGLData()
        {
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            _data = new GLData();
            uint[] VBO = { 0 };
            uint[] VAO = { 0 };
            uint[] EBO = { 0 };
            GL.glGenVertexArrays(1, VAO);

            GL.glGenBuffers(1, VBO);

            GL.glGenBuffers(1, EBO);

			_data.VAO = VAO[0];
			_data.VBO = VBO[0];
			_data.EBO = EBO[0];

            GL.glBindVertexArray(_data.VAO);
            
            GL.glBindBuffer(GL.GL_ARRAY_BUFFER, _data.VBO);

			GL.glBindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, _data.EBO);

            GL.glVertexAttribPointer(0, 3, GL.GL_FLOAT, false, 5 * sizeof(float), IntPtr.Zero);
            GL.glEnableVertexAttribArray(0);
            GL.glVertexAttribPointer(1, 2, GL.GL_FLOAT, false, 5 * sizeof(float), new IntPtr(3 * sizeof(float)));
            GL.glEnableVertexAttribArray(1);
            GL.glEnableVertexAttribArray(0);

            GL.glBindVertexArray(0);
        }

		private void InitializeShaders()
		{
			vertexShader = GL.glCreateShader(GL.GL_VERTEX_SHADER);
            var shaderSource = File.ReadAllText(@"./shaders/shader.vert");

            GL.glShaderSource(vertexShader, 1, new string[] { shaderSource }, new int[] { shaderSource.Length });
            GL.glCompileShader(vertexShader);

            fragmentShader = GL.glCreateShader(GL.GL_FRAGMENT_SHADER);
            shaderSource = File.ReadAllText(@"./shaders/shader.frag");

            GL.glShaderSource(fragmentShader, 1, new string[] { shaderSource }, new int[] { shaderSource.Length });
            GL.glCompileShader(fragmentShader);

            shaderProgram = GL.glCreateProgram();
            GL.glAttachShader(shaderProgram, vertexShader);
            GL.glAttachShader(shaderProgram, fragmentShader);
            GL.glLinkProgram(shaderProgram);
            GL.glUseProgram(shaderProgram);

			GL.glDisable(GL.GL_CULL_FACE);
		}

        private static void InitializeSoundSystem()
        {
#if USE_FMOD_AUDIO
			_soundSystem = new FMODSoundSystem();
#else
            _soundSystem = new SoloudSoundSystem();
#endif
            _soundSystem.Init();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowCursor()
        //------------------------------------------------------------------------------------------------------------------------
        public void ShowCursor(bool enable)
        {
            if (enable)
            {
                GLFW.glfwSetInputMode(Window, GLFW.GLFW_CURSOR, 1);
            }
            else
            {
                GLFW.glfwSetInputMode(Window, GLFW.GLFW_CURSOR, 0);
            }
        }

        public void SetVSync(bool enableVSync)
        {
            _vsyncEnabled = enableVSync;
            GLFW.glfwSwapInterval(_vsyncEnabled ? 1 : 0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetScissor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetScissor(int x, int y, int width, int height)
        {
            if ((width == WindowSize.instance.width) && (height == WindowSize.instance.height))
            {
                GL.glDisable(GL.GL_SCISSOR_TEST);
            }
            else
            {
                GL.glEnable(GL.GL_SCISSOR_TEST);
            }

            GL.glScissor(
                (int)(x * _realToLogicWidthRatio),
                (int)(y * _realToLogicHeightRatio),
                (int)(width * _realToLogicWidthRatio),
                (int)(height * _realToLogicHeightRatio)
            );
            //Glfw.Scissor(x, y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Close()
        //------------------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            _soundSystem.Deinit();
            GLFW.glfwDestroyWindow(Window);
            GLFW.glfwTerminate();
            System.Environment.Exit(0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Run()
        //------------------------------------------------------------------------------------------------------------------------
        public void Run()
        {
            // Console.WriteLine("This is GLContext.cs");
            GLFW.glfwSetTime(0.0);

            do
            {
                if (_vsyncEnabled || (Time.time - _lastFrameTime > (1000 / _targetFrameRate)))
                {
                    _lastFrameTime = Time.time;

                    //actual fps count tracker
                    _frameCount++;
                    if (Time.time - _lastFPSTime > 1000)
                    {
                        _lastFPS = (int)(_frameCount / ((Time.time - _lastFPSTime) / 1000.0f));
                        _lastFPSTime = Time.time;
                        _frameCount = 0;
                    }

                    UpdateMouseInput();
                    _owner.Step();
                    _soundSystem.Step();

                    ResetHitCounters();
                    Display();

                    Time.newFrame();
                    GLFW.glfwPollEvents();

                    // if (GetKey(GLFW.GLFW_KEY_UP)) Console.WriteLine("Up");
                }


                // } while (GLFW.glfwGetWindowAttrib(Window, GLFW.GLFW_FOCUSED) != 0);
            } while (GLFW.glfwWindowShouldClose(Window) != 1);
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														display()
        //------------------------------------------------------------------------------------------------------------------------
        private void Display()
        {

            GL.glClear(GL.GL_COLOR_BUFFER_BIT);

            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();

            _owner.Render(this);

            GLFW.glfwSwapBuffers(Window);
            if (GetKey(Key.ESCAPE)) this.Close();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetColor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetColor(byte r, byte g, byte b, byte a)
        {
            GL.glColor4ub(r, g, b, a);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PushMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PushMatrix(float[] matrix)
        {
            GL.glPushMatrix();
            GL.glMultMatrixf(matrix);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PopMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PopMatrix()
        {
            GL.glPopMatrix();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														DrawQuad()
        //------------------------------------------------------------------------------------------------------------------------
        public void DrawQuad(Vec2[] verts, float[] uvs)
        {
            DrawQuad(verts, new float[16] { 
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f }, uvs);
        }
        public void DrawQuad(Vec2[] verts, float[] transform, float[] uvs)
        {
            verts = AbsoluteToRelative(verts);
            float[] verts_reshaped = {verts[0].x, verts[0].y, 0.0f, uvs[6], uvs[1],
                                      verts[1].x, verts[1].y, 0.0f, uvs[4], uvs[3],
                                      verts[2].x, verts[2].y, 0.0f, uvs[2], uvs[5],
                                      verts[3].x, verts[3].y, 0.0f, uvs[0], uvs[7]
                                      };
			GL.glBindVertexArray(_data.VAO);

            GL.glBufferData(GL.GL_ARRAY_BUFFER, verts_reshaped.Length * sizeof(float), verts_reshaped, GL.GL_STATIC_DRAW);
            GL.glBufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(uint), indices, GL.GL_STATIC_DRAW);

            GL.glUniformMatrix4fv(GL.glGetUniformLocation(shaderProgram, "transform"), 1, false, transform);

			GL.glDrawElements(GL.GL_TRIANGLES, 6, GL.GL_UNSIGNED_INT, IntPtr.Zero);
        }

        public Vec2[] AbsoluteToRelative(Vec2[] verts) 
        {
            var width = WindowSize.instance.width;
            var height = WindowSize.instance.height;
            return verts.Select(v => new Vec2(2.0f*v.x/width - 1.0f, -2.0f*v.y/height + 1.0f)).ToArray();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKey()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKey(int key)
        {
            return keys[key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyDown(int key)
        {
            return keydown[key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyUp(int key)
        {
            return keyup[key];
        }

        public static bool AnyKey()
        {
            return keyPressedCount > 0;
        }

        public static bool AnyKeyDown()
        {
            return anyKeyDown;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButton()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButton(int button)
        {
            return buttons[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonDown(int button)
        {
            return mousehits[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonUp(int button)
        {
            return mouseup[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ResetHitCounters()
        //------------------------------------------------------------------------------------------------------------------------
        public static void ResetHitCounters()
        {
            Array.Clear(keydown, 0, MAXKEYS);
            Array.Clear(keyup, 0, MAXKEYS);
            Array.Clear(mousehits, 0, MAXBUTTONS);
            Array.Clear(mouseup, 0, MAXBUTTONS);
            anyKeyDown = false;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateMouseInput()
        //------------------------------------------------------------------------------------------------------------------------
        public static void UpdateMouseInput()
        {
            GLFW.glfwGetCursorPos(Window, ref mouseX, ref mouseY);
            mouseX = (int)(mouseX / _realToLogicWidthRatio);
            mouseY = (int)(mouseY / _realToLogicHeightRatio);
        }

        public int currentFps
        {
            get { return _lastFPS; }
        }

        public int targetFps
        {
            get { return _targetFrameRate; }
            set
            {
                if (value < 1)
                {
                    _targetFrameRate = 1;
                }
                else
                {
                    _targetFrameRate = value;
                }
            }
        }

    }

}
