namespace App
{
    class Caster
    {
        static void Main(string[] args)
        {
            Map map = CreateMapFromWindow();
            map.AddBorders();
            AddRandObstacles(map, 12, 3);
            map.GetLightSourcePosition();
            RenderLight(map);

            while (true)
            {
                
                UpdateLight(map);
            }

        }

        static Map CreateMapFromInput()
        {
            Console.WriteLine("Map size horizontal:");
            var isNumy = int.TryParse(Console.ReadLine(), out int map_size_y);
            Console.WriteLine("Map size vertical:");

            var isNumx = int.TryParse(Console.ReadLine(), out int map_size_x);

            if (!isNumx || !isNumy)
            {
                Console.WriteLine("Input is not a number");
                CreateMapFromInput();

                return new Map(0, 0);
            }
            else
            {
                Map map = new Map(map_size_x, map_size_y);
                return map;

            }
        }
        static Map CreateMapFromWindow()
        {
            var w = Console.WindowWidth;
            var h = Console.WindowHeight;
            return new Map(h - 1, (w / 2) - 1);
        }

        static void AddRandObstacles(Map map, int num_of_obstacles, int obstacle_size)
        {
            if (map.sizex >= 3 * obstacle_size && map.sizey >= 3 * obstacle_size)
            {
                Random rnd = new Random();
                for (int i = 0; i < num_of_obstacles; i++)
                {
                    int rp1x = rnd.Next(0, map.sizex - obstacle_size);
                    int rp1y = rnd.Next(0, map.sizey - obstacle_size);

                    int rp2x = rp1x + obstacle_size;
                    int rp2y = rp1y + obstacle_size;
                    Point rp1 = new Point(rp1x, rp1y);
                    Point rp2 = new Point(rp2x, rp2y);
                    map.AddObstacle(rp1, rp2);

                }
            }
            else Console.WriteLine("Map too small.");
        }

        static List<Point> Line(Point p0, Point p1)
        {
            List<Point> points = new List<Point>();

            double n = AxialDist(p0, p1);

            for (int i = 0; i <= n; i++)
            {
                double t = (double)i / n;
                points.Add(LerpPoint(p0, p1, t));
            }
            return points;
        }
        static double AxialDist(Point p0, Point p1)
        {
            double dx = Math.Abs(p1.x - p0.x);
            double dy = Math.Abs(p1.y - p0.y);

            return Math.Max(dx, dy);
        }

        static void RenderLight(Map mp)
        {
            var map = (Map)mp.Clone();
            var light_src = map.GetLightSourcePosition();
            var rays = new List<List<Point>>();
            for (int row = 0; row < map.sizex; row++)
            {
                for (int col = 0; col < map.sizey; col++)
                {
                    if (row == 0 || col == 0 || row == map.sizex - 1 || col == map.sizey - 1)
                    {
                        rays.Add(Line(light_src, new Point(col, row)));

                    }
                }
            }
            foreach (var ray in rays)
            {
                map.AddRay(ray, 1);
            }

            map.Display();
        }
        public static void UpdateLight(Map map)
        {

            var source = map.source;
            var sizex = map.sizex;
            var sizey = map.sizey;
            var direction = Console.ReadKey();
            switch (direction.Key)
            {
                case ConsoleKey.A:
                    if (source.x - 1 > 0 && source.x - 1 < sizey - 1)
                        map.source.x -= 1;

                    break;
                case ConsoleKey.D:
                    if (source.x + 1 > 0 && source.x + 1 < sizey - 1)
                        map.source.x += 1;
                    break;
                case ConsoleKey.W:
                    if (source.y - 1 > 0 && source.y - 1 < sizex - 1)

                        map.source.y -= 1;
                    break;
                case ConsoleKey.S:
                    if (source.y + 1 > 0 && source.y + 1 < sizex - 1)

                        map.source.y += 1;
                    break;
                case ConsoleKey.C:
                    map.Display();
                    break;
                case ConsoleKey.Q:
                    System.Environment.Exit(0);
                    break;
                default:
                    break;

            }
            RenderLight(map);

        }

        // (L)inear int(erp)olation 
        static double Lerp(int start, int end, double t)
        {
            return start + t * (end - start);
        }
        static Point LerpPoint(Point p0, Point p1, double t)
        {
            return new Point((int)Lerp(p0.x, p1.x, t), (int)Lerp(p0.y, p1.y, t)
                );
        }

    }
    class Point
    {
        public int x;
        public int y;

        public Point(int x = 0, int y = 0)
        {
            this.x = x; this.y = y;
        }

    }

    class Map : ICloneable
    {
        public int sizex;
        public int sizey;
        public int[,] vals;
        public Point source;
        // Value codes:
        // -1 - light source
        // 0 - void
        // 1 - light
        // 2 - obstacle

        public Map(int sizex, int sizey)
        {
            this.sizex = sizex;
            this.sizey = sizey;

            this.vals = new int[sizex, sizey];
        }
        public void MapFull(int sizex, int sizey, Point source, int[,]vals)
        {
            this.sizex = sizex;
            this.sizey = sizey;
            this.source = source;
            this.vals = vals;
        }

        public Map AddRay(List<Point> point_list, int value)
        {
            var temp_map = this;
            foreach (Point p in point_list)
            {
                if (p.x > 0 && p.y > 0 && p.y < sizex && p.x < sizey)
                {
                    if (temp_map.vals[p.y, p.x] != 2)
                    {

                        try
                        {
                            if (p != temp_map.source) { temp_map.vals[p.y, p.x] = value; }

                        }
                        catch (IndexOutOfRangeException)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return temp_map;
        }

        public Point GetLightSourcePosition()
        {
            if (source != null)
            {
                return source;
            }
            else
            {
                source = new Point(sizey / 2, sizex / 2);
                return source;

            }

        }
        public void UpdateLightSourcePosition()
        {
            vals[source.y, source.x] = -1;
        }

        public void Display()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            this.UpdateLightSourcePosition();
            for (int row = 0; row < sizex; row++)
            {
                for (int col = 0; col < sizey; col++)
                    switch (vals[row, col])
                    {
                        case -1:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(String.Format("[]", vals[row, col]));

                            break;
                        case 0:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(String.Format("  ", vals[row, col]));

                            break;

                        case 1:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(String.Format("::", vals[row, col]));

                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(String.Format("##", vals[row, col]));

                            break;
                        default:
                            Console.Write(String.Format("{0}", vals[row, col]));
                            break;

                    }
                Console.WriteLine();
            }
        }
        bool IsValid(Point p)
        {
            return Enumerable.Range(0, sizex).Contains(p.x) && Enumerable.Range(0, sizey).Contains(p.y);
        }
        public void AddBorders()
        {

            for (int row = 0; row < sizex; row++)
            {
                for (int col = 0; col < sizey; col++)
                {
                    if (row == 0 || col == 0 || row == sizex - 1 || col == sizey - 1) { vals[row, col] = 2; }
                }
            }
        }
        public void AddObstacle(Point p0, Point p1)
        {

            if (IsValid(p0) && IsValid(p1))
            {
                for (int i = 0; i < sizex; i++)
                {
                    for (int j = 0; j < sizey; j++)
                    {
                        if ((
                            (i >= p0.x && i <= p1.x) ||
                            (i <= p0.x && i >= p1.x)
                            ) && (
                            (j >= p0.y && j <= p1.y) ||
                            (j <= p0.y && j >= p1.y)
                            ))
                        {
                            vals[i, j] = 2;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid Points");
            }
        }
        public object Clone()
        {
            var empty = new Map(sizex, sizey);
        
            empty.MapFull(sizex, sizey, source, (int[,])vals.Clone());
            return empty;
        }
    }
}