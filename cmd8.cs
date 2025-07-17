using System;
using System.Windows;
using System.IO;
using System.Globalization; 
using AlibreAddOn;

public class cmd8 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite commandSite;
    private bool isOutOfDate = true;


    private float[] vertices = new float[] { -5, 0, 0, 5, 0, 0, 0, 5, 0 };
    private float[] normals = new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 };
    private int[] displayIndices = new int[] { 3, 0, 1, 2 }; 

    public IADAddOnCommandSite CommandSite
    {
        get => commandSite;
        set => commandSite = value;
    }

    public void putref_CommandSite(IADAddOnCommandSite pSite)
    {
        commandSite = pSite;
        bool isLegacy = commandSite.LegacyRenderingEngine();
        if (isLegacy)
            throw new InvalidOperationException("");
    }

    public bool AddTab() => false;
    public string TabName => null;
    public Array Extents => null;
    public void OnShowUI(long hWnd) { }

    public void OnComplete()
    {
        isOutOfDate = true;
        commandSite.InvalidateCanvas();
    }
    public void On3DRender()
    {
        if (!isOutOfDate) return;

        object canvasObj = commandSite.Begin3DDisplay(false);
        var canvas = (IADAddOnCanvasDisplay)canvasObj;

        DrawPreviewTriangle(canvas);

        commandSite.End3DDisplay();
        isOutOfDate = false;
    }

    private void DrawPreviewTriangle(IADAddOnCanvasDisplay canvas)
    {
        Array transform = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 };
        Array verts = vertices;
        Array norms = normals;
        Array inds = displayIndices;

        long segment = canvas.AddSubSegment(0, "HOOPS_Preview_Triangle");
        canvas.SetSegmentTransform(segment, true, ref transform);
        canvas.SetSegmentColor(segment, 255, 128, 0, 255); // Orange
        canvas.DrawMesh(segment, ref verts, ref norms, ref inds);
    }
    public void OnDoubleClick(int screenX, int screenY)
    {
        var result = MessageBox.Show(
            "Do you want to generate a real part from this mesh?", 
            "Generate Geometry", 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            GenerateAndImportTriangle();
            OnEscape();
        }
    }

    private void GenerateAndImportTriangle()
    {
        string stlPath = Path.Combine(Path.GetTempPath(), "alibre_addon_mesh.stl");
        
        try
        {
            using (StreamWriter writer = new StreamWriter(stlPath))
            {
                writer.WriteLine("solid MyTriangle");
                writer.WriteLine($"  facet normal 0 0 1");
                writer.WriteLine("    outer loop");
                writer.WriteLine($"      vertex {vertices[0].ToString(CultureInfo.InvariantCulture)} {vertices[1].ToString(CultureInfo.InvariantCulture)} {vertices[2].ToString(CultureInfo.InvariantCulture)}");
                writer.WriteLine($"      vertex {vertices[3].ToString(CultureInfo.InvariantCulture)} {vertices[4].ToString(CultureInfo.InvariantCulture)} {vertices[5].ToString(CultureInfo.InvariantCulture)}");
                writer.WriteLine($"      vertex {vertices[6].ToString(CultureInfo.InvariantCulture)} {vertices[7].ToString(CultureInfo.InvariantCulture)} {vertices[8].ToString(CultureInfo.InvariantCulture)}");
                writer.WriteLine("    endloop");
                writer.WriteLine("  endfacet");
                writer.WriteLine("endsolid MyTriangle");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to generate or import mesh: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            if (File.Exists(stlPath))
            {
                File.Delete(stlPath);
            }
        }
    }


    public void OnEscape()
    {
        commandSite.Override3DRender(false);
        commandSite.InvalidateCanvas();
        commandSite.Terminate();
    }
    public void OnClick(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnMouseDown(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnMouseMove(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnMouseUp(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnMouseWheel(double delta) { }
    public void OnKeyDown(int keyCode) { }
    public void OnKeyUp(int keyCode) { }
    public void OnSelectionChange() { }
    public void OnTerminate() { }
    public bool IsTwoWayToggle() => false;
    public void OnRender(int hDC, int clipRectX, int clipRectY, int clipRectWidth, int clipRectHeight)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnClick(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnDoubleClick(int screenX, int screenY)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseDown(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseMove(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseUp(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnKeyDown(int keycode)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnKeyUp(int keycode)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnEscape()
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseWheel(double delta)
    {
        throw new NotImplementedException();
    }

 
}