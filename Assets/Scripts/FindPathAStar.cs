using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathMarker
{
    public MapLocation location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;
  


    public PathMarker(MapLocation l, float g, float h, GameObject marker, PathMarker p)
    {
        location = l;
        G = g;
        H = h;
        F = g + h;
        this.marker = marker;
        parent = p;
    }
    public PathMarker(MapLocation l, float g, float h, float f, GameObject marker, PathMarker p)
    {
        location = l;
        G = g;
        H = h;
        F = f;
        this.marker = marker;
        parent = p;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return location.Equals(((PathMarker) obj).location);
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}


public class FindPathAStar : MonoBehaviour
{
    private bool canGetPath;
    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;

    List<PathMarker> open = new List<PathMarker>();   //Lista OPEN de PathMakers
    List<PathMarker> closed = new List<PathMarker>(); //Lista CLOSED de PathMakers

    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    public float updateFreq = 0.2f;

    PathMarker goalNode;  //PathMaker que marca la posicion final (DESTINO)
    PathMarker startNode; //PathMaker que marca la posicion inicial (ORIGEN)

    PathMarker lastPos; //Nodo en el que estamos en cada momento, cuando estamos
                        //buscando el DESTINO y cuando estamos dibujando el CAMINO
    bool done = false;  //Booleano que se utiliza para decir si se ha alcanzado el objetivo

    bool algorithmStarted = false;  //Hemos inicializado el juego

    void RemoveAllMarkers()
    {
        //TODO: Eliminar todos los GameObjects que puedan haber en el mapa (maze)
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject m in markers)
            Destroy(m);
    }

    void BeginSearch()
    {
        //Aquн inicializamos las posiciones inicial, final y las listas OPEN y CLOSED

        //Esta variable controlarб si el problema estб solucionado
        done = false;
        //Eliminar posibles GameObjects en el mapa
        RemoveAllMarkers();

        algorithmStarted = true;

        //Creamos una lista de localizaciones validas, esto es, objetos del tipo MapLocation
        List<MapLocation> locations = new List<MapLocation>();
        //Ahora generamos el grafo comprobando si, en cada posiciуn, tenemos un muro o no (-1 o 1)
        // En el caso de no tener un muro, agregamos un nuevo MapLocation con esa coordenada a la lista
        for (int z = 1; z < maze.depth - 1; z++)
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x, z] != 1)
                    locations.Add(new MapLocation(x, z));
            }
        //Las listas tienen un mйtodo Shuffle que nos permite barajar los elementos que contiene
        // Barajamos las posiciones para que no estйn contiguas
        locations.Shuffle();

        //Escogemos una localizaciуn al azar (la que se quedу en la primera posiciуn, por ejemplo) y esa serб 
        // la posiciуn de salida, almacenada en un Vector3
        // Cuidado: Para mantener la ubicaciуn consistente, tenemos que multiplicar la coordenada por la escala del laberinto (maze.scale)
        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        //Ahora podemos crear un nuevo PathMarker, el objeto que utitlizaremos para representar una posiciуn dentro del laberinto de forma visual
        //Un PathMarker necesita:
        // - MapLocation
        // - Los valores de la funciуn f:
        //      - g(n)
        //      - h(n)
        // - El GameObject con el que representaremos esta posiciуn
        // - Y otro PathMarker que serб el "padre" de este nodo

        //Inicializamos goalNode como posicion de llegada, creando de la misma forma un Vector3 con las coordenadas, 
        // y despuйs un PathMarker con esta informaciуn
        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0, Instantiate(end, goalLocation, Quaternion.identity), null);

        // Este nodo empieza con su valor de la heurнstica (distancia euclidea hasta el objetivo).
        float h_n = Vector2.Distance(locations[0].ToVector(), goalNode.location.ToVector());
        GameObject startBlock = Instantiate(start, startLocation, Quaternion.identity);
        
        startNode = new PathMarker(locations[0], 0, h_n, startBlock, null);
        
        TextMesh[] values = startBlock.GetComponentsInChildren<TextMesh>();
        values[0].text = "g(n): " + startNode.G.ToString("0.00");
        values[1].text = "h(n): " + startNode.H.ToString("0.00");
        values[2].text = "f(n): " + startNode.F.ToString("0.00");
        

        //Eliminamos lo que contenga las listas de OPEN y CLOSED por si se ha vuelto a ejecutar el algoritmo
        open.Clear();
        closed.Clear();

        //Comenzamos aсadiendo el nodo startNode a CLOSED
        closed.Add(startNode); 

        //Y para cuestiones relacionadas con la UI, almacenamos una referencia al nodo inicial en lastPos, por ahora
        lastPos = startNode;
    }

    //TODO: Completad este mйtodo
    void Search(PathMarker thisNode)
    {
        // 1. Si hemos llegado al destino, terminamos la búsqueda
        if (thisNode.Equals(goalNode))
        {
            done = true;
            CancelInvoke();
            canGetPath = true;
            //GetPath();
            return;
        }

        // 2. Movemos el nodo actual de OPEN a CLOSED
        open.Remove(thisNode);
        closed.Add(thisNode);

        // Cambiamos el color al material de CLOSED
        if (thisNode.marker != null)
        {
            Renderer rend = thisNode.marker.GetComponent<Renderer>();
            if (rend != null && closedMaterial != null)
                rend.material = closedMaterial;
        }

        // 3. Comprobamos sus cuatro vecinos
        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = thisNode.location + dir;

            // Ignoramos posiciones que no sean transitables
            if (maze.map[neighbour.x, neighbour.z] != 0)
                continue;

            // Ignoramos nodos que ya hemos visitado
            if (IsClosed(neighbour))
                continue;

            // 4. Calculamos los costes para el vecino
            float g = thisNode.G + 1;

            float h = Vector2.Distance(
                new Vector2(neighbour.x, neighbour.z),
                new Vector2(goalNode.location.x, goalNode.location.z)
            );

            float f = g + h;

            // 5. Si ya está en OPEN, actualizamos si encontramos
            // una ruta más barata. Si no está, creamos un nodo nuevo.
            if (UpdateMarker(neighbour, g, h, f, thisNode))
            {
                continue;
            }

            // 6. Creamos el marcador del nuevo nodo
            GameObject marker = Instantiate(
                pathP,
                new Vector3(neighbour.x * maze.scale, 0, neighbour.z * maze.scale),
                Quaternion.identity
            );

            marker.tag = "marker";

            // Asignamos el material de OPEN
            Renderer rend = marker.GetComponent<Renderer>();
            if (rend != null && openMaterial != null)
                rend.material = openMaterial;

            PathMarker newNode = new PathMarker(
                neighbour,
                g,
                h,
                f,
                marker,
                thisNode
            );

            open.Add(newNode);

            // 7. Mostramos los costes en los TextMesh del marcador
            TextMesh[] values = marker.GetComponentsInChildren<TextMesh>();

            if (values.Length >= 3)
            {
                values[0].text = "g(n): " + g.ToString("0.00");
                values[1].text = "h(n): " + h.ToString("0.00");
                values[2].text = "f(n): " + f.ToString("0.00");
            }
        }

        // 8. Seleccionamos el nodo con menor F para la siguiente iteración
        if (open.Count > 0)
        {
            open.Sort((a, b) => a.F.CompareTo(b.F));
            lastPos = open[0];
        }
        else
        {
            // No quedan nodos por explorar: no hay camino
            done = true;
            CancelInvoke();
        }
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        //Para cada pathMaker en al open list
        //   Si pos esta en la lista:
        //      actualizamos G H F y parent
        //      devolvemos TRUE
        // 5. Si el vecino ya está en OPEN, solo actualizamos
        // cuando hemos encontrado una ruta más barata

        
            // Recorremos los nodos que ya están en OPEN
            foreach (PathMarker p in open)
            {
                // Comprobamos si ya existe esta posición
                if (p.location.Equals(pos))
                {
                    // Solo actualizamos si la nueva ruta es más barata
                    if (g < p.G)
                    {
                        p.G = g;
                        p.H = h;
                        p.F = f;
                        p.parent = prt;
                    }

                    // La posición ya estaba en OPEN
                    return true;
                }
            }

            // No hemos encontrado la posición en OPEN
            return false;
        

    }

    bool IsClosed(MapLocation marker)
    {
        //Devolvemos TRUE si marker esta en la lista de closed
        foreach(PathMarker p in closed)
        {
            if (p.location.Equals(marker)) return true;
        }
        return false;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }



    //TODO: Completad este mйtodo
    void GetPath()
    {
        // 1. Comprobamos que se haya encontrado el destino
        if (!done || lastPos == null ||
            !lastPos.location.Equals(goalNode.location))
        {
            Debug.LogWarning(
                "No se puede dibujar el camino: A* no ha llegado al destino."
            );
            return;
        }

        // 2. Eliminamos los marcadores anteriores
        RemoveAllMarkers();

        // 3. Empezamos desde el destino
        PathMarker begin = lastPos;

        // 4. Retrocedemos mediante los padres hasta el inicio
        while (begin != null &&
               !begin.location.Equals(startNode.location))
        {
            // No dibujamos un pathBlock encima del destino
            if (!begin.location.Equals(goalNode.location))
            {
                Vector3 pos = new Vector3(
                    begin.location.x * maze.scale,
                    0,
                    begin.location.z * maze.scale
                );

                Instantiate(pathP, pos, Quaternion.identity);
            }

            // Retrocedemos al nodo anterior
            begin = begin.parent;
        }

        // 5. Volvemos a dibujar el inicio
        Vector3 startLocation = new Vector3(
            startNode.location.x * maze.scale,
            0,
            startNode.location.z * maze.scale
        );

        Instantiate(start, startLocation, Quaternion.identity);

        // 6. Volvemos a dibujar el destino
        Vector3 goalLocation = new Vector3(
            goalNode.location.x * maze.scale,
            0,
            goalNode.location.z * maze.scale
        );

        Instantiate(end, goalLocation, Quaternion.identity);
    }

    void Search()
    {
        Search(lastPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null) return;

        //Si presionamos la letra "P" inicializamos el juego
        if (Keyboard.current[Key.P].wasPressedThisFrame)
        {
            CancelInvoke();
            BeginSearch();
        }

        //Si presionamos la letra "C" calculamos el siguiente movimiento del alg. A*
        if (Keyboard.current[Key.C].wasPressedThisFrame && !done)
        {
            Search(lastPos);
        }
        //Si presionamos la letra "M" dejamos solo los pathMakers que forman el path optimo
        if (Keyboard.current[Key.M].wasPressedThisFrame && canGetPath)
        {
            GetPath();
        }

        //Si presionamos la tecla A, se reproduce la bъsqueda a modo animaciуn con una frecuencia dada
        if (Keyboard.current[Key.A].wasPressedThisFrame)
        {
            CancelInvoke();
            InvokeRepeating(nameof(Search), 0.5f, updateFreq);
        }
    }
}