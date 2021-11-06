using System;
using SDL2;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using System.Numerics;
using ImGuiBackends.Rendering;
using OpenTKImGuiBackends.Interaction;

namespace OpenGLApp
{
    public class SDLBindingsContext : OpenTK.IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return SDL.SDL_GL_GetProcAddress(procName);
        }
    }

    class Program
    {
        static Vector4 Gray = new Vector4(0.7f, 0.7f, 0.7f, 1);

        static void Main(string[] args)
        {
            var win = SDL.SDL_CreateWindow(
                "OpenTK ImGui Application",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                1280,
                720,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN |
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK,
                (int)SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

            var sdlCtx = SDL.SDL_GL_CreateContext(win);

            GL.LoadBindings(new SDLBindingsContext());

            var imguiCtx = ImGui.CreateContext();
            ImGui.SetCurrentContext(imguiCtx);

            var glVersion = GL.GetString(StringName.Version);
            var glRenderer = GL.GetString(StringName.Renderer);
            var glVendor = GL.GetString(StringName.Vendor);
            var glGlslVersion = GL.GetString(StringName.ShadingLanguageVersion);
            var glExtensions = GL.GetString(StringName.Extensions);

            Console.WriteLine("ImGui backend OpenGL+SDL2 application");
            Console.WriteLine($"OpenGL version: {glVersion}");
            Console.WriteLine($"On: {glRenderer}");
            Console.WriteLine($"Vendor: {glVendor}");
            Console.WriteLine($"GLSL version: {glGlslVersion}");

            var igIo = ImGui.GetIO();
            igIo.DisplaySize.X = 1280;
            igIo.DisplaySize.Y = 720;

            ImGuiImplSDL2.Init(win);
            ImGuiImplOpenGL3.Init();

            var open = true;

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            while (open)
            {
                while (SDL.SDL_PollEvent(out SDL.SDL_Event evt) != 0)
                {
                    ImGuiImplSDL2.ProcessEvent(evt);
                    switch (evt.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            open = false;
                            break;
                    }
                }

                var elapsedTime = sw.Elapsed;
                var deltaTime = (float)elapsedTime.TotalSeconds;
                sw.Restart();

                ImGuiImplSDL2.NewFrame();
                ImGui.NewFrame();

                ImGui.ShowDemoWindow();

                if (ImGui.Begin("Hello, OpenTK world!"))
                {
                    ImGui.Text("Hello world, OpenTK! This is using OpenTKImGuiBackends.");
                    ImGui.NewLine();
                    ImGui.TextColored(Gray, "Information");
                    ImGui.Text($"OpenGL version: {glVersion}");
                    ImGui.Text($"On: {glRenderer}");
                    ImGui.Text($"Vendor: {glVendor}");
                    ImGui.Text($"GLSL version: {glGlslVersion}");
                    ImGui.Text($"Extensions: {glExtensions}");
                    ImGui.NewLine();
                    ImGui.TextColored(Gray, "~ 2021 Rin https://github.com/ry00001");
                    ImGui.End();
                }

                GL.ClearColor(0, 0, 0, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit |
                         ClearBufferMask.DepthBufferBit);

                ImGui.Render();
                ImGuiImplOpenGL3.Render();

                SDL.SDL_GL_SwapWindow(win);
            }

            SDL.SDL_DestroyWindow(win);
        }
    }
}
