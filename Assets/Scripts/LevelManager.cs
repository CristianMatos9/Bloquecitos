using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Fichas del tablero y fichas negras")]
    public GameObject[] fichasNegras;  // Array de fichas negras
    public GameObject[] fichasTablero; // Array de fichas del tablero

    void Start()
    {
        ColocarFichas();
    }

    private void ColocarFichas()
    {
        foreach (GameObject fichaNegra in fichasNegras)
        {
            if (fichaNegra != null)
            {
                GameObject fichaTableroMasCercana = EncontrarFichaTableroMasCercana(fichaNegra);
                if (fichaTableroMasCercana != null)
                {
                    fichaNegra.transform.position = fichaTableroMasCercana.transform.position;
                }
            }
        }
    }

    private GameObject EncontrarFichaTableroMasCercana(GameObject fichaNegra)
    {
        GameObject fichaMasCercana = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (GameObject fichaTablero in fichasTablero)
        {
            if (fichaTablero != null)
            {
                float distancia = Vector2.Distance(fichaNegra.transform.position, fichaTablero.transform.position);
                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    fichaMasCercana = fichaTablero;
                }
            }
        }
        return fichaMasCercana;
    }
}