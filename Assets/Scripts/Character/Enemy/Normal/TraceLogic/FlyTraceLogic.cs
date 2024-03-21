using System.Collections;
using System.Collections.Generic;
using Character.Enemy.Normal;
using Enemy;
using Enemy.TraceLogic;
using UnityEngine;

public class FlyTraceLogic : EnemyTraceLogic
{
    private readonly float amplitudeMax;
    private readonly float waveLengthMax;
    private readonly float traceSpeed;
    private readonly Vector2 targetPos;
    private float waveLenghtCur = 0;
    private bool goingUp = true;

    private Vector2 traceDir;
    public FlyTraceLogic(float traceSpeed, float amplitude, float waveLenght, Vector2 targetPos)
    {
        this.traceSpeed = traceSpeed;
        this.amplitudeMax = amplitude;
        this.waveLengthMax = waveLenght;
        this.targetPos = targetPos;
    }

    public override Vector2 GetTraceDir(EnemyController enemyController)
    {
        Vector2 playerPos = enemyController.player.transform.position;
        Vector2 enemyPos = enemyController.transform.position;
        Vector2 toPlayerDir;

        if (playerPos.x < enemyPos.x)
            toPlayerDir = playerPos + targetPos - enemyPos;
        else
            toPlayerDir = new Vector2(playerPos.x - targetPos.x - enemyPos.x, playerPos.y + targetPos.y - enemyPos.y);

        float nowAmp = getAplitudeCur(enemyPos, toPlayerDir) * ((waveLenghtCur / waveLengthMax) - 0.5f);
        traceDir = toPlayerDir.normalized * traceSpeed;
        traceDir += Vector2.Perpendicular(toPlayerDir).normalized * nowAmp;
        traceDir = traceDir.normalized * traceSpeed;
        
        if (waveLenghtCur > waveLengthMax)
            goingUp = false;
        else if (waveLenghtCur < 0)
            goingUp = true;
        if (goingUp)
            waveLenghtCur += Time.deltaTime;
        else
            waveLenghtCur -= Time.deltaTime;

        return traceDir;
    }

    public override void DrawDebug(EnemyController enemyController)
    {
        var position = enemyController.transform.position;
        Debug.DrawLine(position, position + (Vector3)traceDir, Color.blue);
    }

    public float getAplitudeCur(Vector2 pos, Vector2 dir)
    {
        Vector2 rayPos = new Vector2(pos.x - dir.normalized.x, pos.y - dir.normalized.y);
        RaycastHit2D rightSide = Physics2D.Raycast(rayPos, Vector2.Perpendicular(dir), amplitudeMax, LayerMask.GetMask("Platform"));
        RaycastHit2D leftSide = Physics2D.Raycast(rayPos, Vector2.Perpendicular(dir) * -1, amplitudeMax, LayerMask.GetMask("Platform"));
        if (rightSide.collider != null && leftSide.collider != null)
            return Mathf.Max(rightSide.distance, leftSide.distance);
        else if (rightSide.collider == null && leftSide.collider == null)
            return amplitudeMax;
        else
            return rightSide.distance + leftSide.distance;
    }
}
